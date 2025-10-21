using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tiled;
using Newtonsoft.Json;
using nkast.Aether.Physics2D.Diagnostics;
using nkast.Aether.Physics2D.Dynamics;
using Platformer.Component;
using Platformer.ContactListeners;
using Platformer.Factories;
using Platformer.Models;
using Platformer.Systems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using World = MonoGame.Extended.ECS.World;

namespace Platformer
{
    public class PlatformerGame : Game
    {
        private DebugView _debugView;
        private PhysicsSystem _physicsSystem;
        private World _world;

        private OrthographicCamera _camera;
        private SpriteBatch _worldSpriteBatch;
        private SpriteBatch _uiSpriteBatch;
        private Matrix _projectionMatrix;
        private Matrix _scaleMatrix;
        private Matrix _reflectionMatrix = Matrix.CreateReflection(new Plane(0f, 1f, 0f, 0f));

        private MyTiledMapRenderer _tiledRenderer;

        public PlatformerGame()
        {
            Window.AllowUserResizing = true;
            _ = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 960
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _camera = new OrthographicCamera(GraphicsDevice);
            _worldSpriteBatch = new SpriteBatch(GraphicsDevice);
            _uiSpriteBatch = new SpriteBatch(GraphicsDevice);
            _physicsSystem = new PhysicsSystem();
            _world = new WorldBuilder()
                .AddSystem( new SpriteDrawSystem(_worldSpriteBatch))
                .AddSystem(new PlayerInputSystem(this))
                .AddSystem(_physicsSystem)
                .AddSystem(new CameraTargetSystem(_camera))
                .Build();

            _physicsSystem.RegisterContactListener(new OneWayContactListener(_world));
            _physicsSystem.RegisterContactListener(new GroundedContactListener(_world));
            _physicsSystem.RegisterContactListener(new SpringContactListener(_world));
            _debugView = new DebugView(_physicsSystem.PhysicsWorld);
            _debugView.AppendFlags(DebugViewFlags.Shape | DebugViewFlags.ContactPoints | DebugViewFlags.ContactNormals);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            const string mapName = "MoveAndJump";
            TiledMap tiledMap = Content.Load<TiledMap>(mapName);
            _tiledRenderer = new MyTiledMapRenderer(GraphicsDevice, tiledMap);

            _projectionMatrix = Matrix.CreateOrthographic(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0.0f, 1.0f);
            _scaleMatrix = Matrix.CreateScale(tiledMap.TileWidth, tiledMap.TileHeight, 1.0f);

            foreach (var mapObject in tiledMap.ObjectLayers.SelectMany(l => l.Objects))
            {
                Entity entity = _world.CreateEntity();
                if (mapObject is TiledMapTileObject tileObject && tileObject.Tile != null)
                    CreateComponentsFromProperties(tiledMap, tileObject, tileObject.Tile.Properties, entity);
                else
                    CreateComponentsFromProperties(tiledMap, mapObject, mapObject.Properties, entity);
            }

            _debugView.LoadContent(GraphicsDevice, Content);
        }

        private void CreateComponentsFromProperties(TiledMap map, TiledMapObject mapObject, TiledMapProperties properties, Entity entity)
        {
            var bodyFactory = new TiledBodyFactory(map.GetScale());
            var spriteFactory = new AnimatedSpriteFactory(Content);
            foreach (var prop in properties)
            {
                switch (prop.Key)
                {
                    case nameof(BodyType):
                        var bodyType = Enum.Parse<BodyType>(prop.Value);
                        Vector2 position = (mapObject.Position.ToPoint() + mapObject.Size / 2f) * map.GetScale();
                        var rotation = mapObject.Rotation * (float)Math.PI / 180f;

                        Body body = _physicsSystem.PhysicsWorld.CreateBody(position, rotation, bodyType);
                        if (properties.TryGetValue("FixedRotation", out string fixedRotation))
                            body.FixedRotation = bool.Parse(fixedRotation);
                        bodyFactory.BuildFixturesFromTiledObject(mapObject, body);
                        body.Tag = entity.Id;
                        entity.Attach(body);
                        break;
                    case nameof(CameraTarget):
                        var cameraTarget = JsonConvert.DeserializeObject<CameraTarget>(prop.Value);
                        cameraTarget.CameraBounds = new(0, 0, map.WidthInPixels, map.HeightInPixels);
                        cameraTarget.ScaleMatrix = Matrix.CreateScale(map.TileWidth, map.TileHeight, 1f);
                        entity.Attach(cameraTarget);
                        break;
                    case nameof(KeyboardController):
                        entity.Attach(JsonConvert.DeserializeObject<KeyboardController>(prop.Value));
                        break;
                    case nameof(DebugController):
                        entity.Attach(JsonConvert.DeserializeObject<DebugController>(prop.Value));
                        break;
                    case nameof(OneWayPlatform):
                        entity.Attach(JsonConvert.DeserializeObject<OneWayPlatform>(prop.Value));
                        break;
                    case nameof(SpringComponent):
                        entity.Attach(JsonConvert.DeserializeObject<SpringComponent>(prop.Value));
                        break;
                    case nameof(AnimatedSprite):
                        var model = Content.Load<AnimatedSpriteModel>(prop.Value);
                        entity.Attach(spriteFactory.BuildAnimatedSprite(model));
                        break;
                    default:
                        Debug.WriteLine($"No such component: {prop.Key}");
                        break;
                }
            }
            if (mapObject is TiledMapTileObject tileObject && tileObject.Tile != null)
            {
                var id = tileObject.Tile is TiledMapTilesetAnimatedTile animated
                    ? animated.CurrentAnimationFrame.LocalTileIdentifier
                    : tileObject.Tile.LocalTileIdentifier;
                var tileRegion = tileObject.Tileset.GetTileRegion(id);
                var texture = tileObject.Tileset.Texture;
                var textureRegion = new Texture2DRegion(texture, tileRegion.X, tileRegion.Y, tileRegion.Width, tileRegion.Height, 
                    false, tileRegion.Size, Vector2.Zero, Vector2.One * 0.5f, tileObject.Name);
                entity.Attach(new Sprite(textureRegion));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            _tiledRenderer.Update(gameTime);
            _world.Update(gameTime);

            UpdateDebug();

            _debugView.UpdatePerformanceGraph(gameTime.ElapsedGameTime);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //draw tiled background
            _tiledRenderer.DrawBackgroundLayers(_camera.GetViewMatrix());

            _worldSpriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
            _world.Draw(gameTime);
            _worldSpriteBatch.End();

            //draw tiled foreground
            _tiledRenderer.DrawForegroundLayers(_camera.GetViewMatrix());

            _uiSpriteBatch.Begin();
            //draw the ui
            _uiSpriteBatch.End();

            var translation = Matrix.CreateTranslation(-GraphicsDevice.Viewport.Width / 2f, -GraphicsDevice.Viewport.Height /2f, 0f);
            _debugView.RenderDebugData(_projectionMatrix, _scaleMatrix * _camera.GetViewMatrix() * translation * _reflectionMatrix);

            base.Draw(gameTime);
        }

        internal Vector2 GetWorldCoordinates(float x, float y) => _camera.ScreenToWorld(x, y);
        internal IEnumerable<Body> GetBodiesAt(float x, float y) => _physicsSystem.GetBodiesAt(x, y);

        private void UpdateDebug()
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                Vector2 worldMouse = GetWorldCoordinates(mouse.X, mouse.Y);
                var bodiesUnderMouse = GetBodiesAt(worldMouse.X, worldMouse.Y);
                foreach (var body in bodiesUnderMouse)
                {
                    System.Diagnostics.Debug.WriteLine($"---------entity: {body.Tag}");
                    System.Diagnostics.Debug.WriteLine($"          local: {body.GetLocalPoint(worldMouse)}");
                    System.Diagnostics.Debug.WriteLine($"       Position: {body.Position}");
                    System.Diagnostics.Debug.WriteLine($"      IsEnabled: {body.Enabled}");
                    System.Diagnostics.Debug.WriteLine($"        IsAwake: {body.Awake}");
                    System.Diagnostics.Debug.WriteLine($"           Mass: {body.Mass}");
                    System.Diagnostics.Debug.WriteLine($" LinearVelocity: {body.LinearVelocity}");
                    System.Diagnostics.Debug.WriteLine($"AngularVelocity: {body.AngularVelocity}");
                    System.Diagnostics.Debug.WriteLine($"-------Fixtures: {body.FixtureList.Count}");

                    foreach (var fixture in body.FixtureList)
                    {
                        System.Diagnostics.Debug.WriteLine($"        Fixture: {fixture.Shape.ShapeType}");
                        System.Diagnostics.Debug.WriteLine($"        Density: {fixture.Shape.Density}");
                        System.Diagnostics.Debug.WriteLine($"       Friction: {fixture.Friction}");
                        System.Diagnostics.Debug.WriteLine($"    Restitution: {fixture.Restitution}");
                    }
                }
            }
        }

    }
}