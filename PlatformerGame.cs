using nkast.Aether.Physics2D.Dynamics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tiled;
using Newtonsoft.Json;
using Platformer.Component;
using Platformer.Factories;
using Platformer.Systems;
using System;
using System.Linq;
using World = MonoGame.Extended.ECS.World;
using Platformer.Models;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Platformer.ContactListeners;

namespace Platformer
{
    public class PlatformerGame : Game
    {
        internal TiledMapRenderSystem _renderSystem;
        private PhysicsSystem _physicsSystem;
        private World _world;

        public PlatformerGame()
        {
            Window.AllowUserResizing = true;
            var graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 960
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _physicsSystem = new PhysicsSystem();
            var spriteBatch = new SpriteBatch(GraphicsDevice);
            _renderSystem = new TiledMapRenderSystem(spriteBatch);
#if DEBUG
            _renderSystem.PhysicsDebugDrawer = new Box2dDebugDrawer(_physicsSystem.PhysicsWorld, spriteBatch);
#endif
            _world = new WorldBuilder()
                .AddSystem(_renderSystem)
                .AddSystem(new PlayerInputSystem(this))
                .AddSystem(_physicsSystem)
                .Build();

            _physicsSystem.RegisterContactListener(new OneWayContactListener(_world));
            _physicsSystem.RegisterContactListener(new GroundedContactListener(_world));
            _physicsSystem.RegisterContactListener(new SpringContactListener(_world));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            const string mapName = "MoveAndJump";
            TiledMap tiledMap = Content.Load<TiledMap>(mapName);
            _renderSystem.SetTiledMap(tiledMap);

            foreach (var mapObject in tiledMap.ObjectLayers.SelectMany(l => l.Objects))
            {
                Entity entity = _world.CreateEntity();
                if (mapObject is TiledMapTileObject tileObject && tileObject.Tile != null)
                {
                    entity.Attach(tileObject);
                    CreateComponentsFromProperties(tileObject, tileObject.Tile.Properties, entity, tiledMap.GetScale());
                }

                CreateComponentsFromProperties(mapObject, mapObject.Properties, entity, tiledMap.GetScale());
            }
        }

        private void CreateComponentsFromProperties(TiledMapObject mapObject, TiledMapProperties properties, Entity entity, Vector2 scale)
        {
            var bodyFactory = new TiledBodyFactory(scale);
            var spriteFactory = new AnimatedSpriteFactory(Content);
            foreach (var prop in properties)
            {
                switch (prop.Key)
                {
                    case nameof(BodyType):
                        var bodyType = Enum.Parse<BodyType>(prop.Value);
                        Vector2 position = mapObject.Position.ToNumerics();
                        position.X += mapObject.Size.Width / 2f;
                        position.Y -= mapObject.Size.Height / 2f;
                        position *= scale;
                        var rotation = mapObject.Rotation * (float)Math.PI / 180f;

                        Body body = _physicsSystem.PhysicsWorld.CreateBody(position, rotation, bodyType);
                        bodyFactory.BuildFixturesFromTiledObject(mapObject, body);
                        body.Tag = entity.Id;
                        entity.Attach(body);
                        break;
                    case nameof(CameraTarget):
                        entity.Attach(JsonConvert.DeserializeObject<CameraTarget>(prop.Value));
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
                        //var model = JsonConvert.DeserializeObject<Models.AnimatedSpriteModel>(json);

                        entity.Attach(spriteFactory.BuildAnimatedSprite(model));
                        break;
                    default:
                        Debug.WriteLine($"No such component: {prop.Key}");
                        break;
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            _world.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _world.Draw(gameTime);

            base.Draw(gameTime);
        }

        internal Vector2 GetWorldCoordinates(float x, float y) => _renderSystem.GetWorldCoordinates(x, y);

        internal IEnumerable<Body> GetBodiesAt(float x, float y) => _physicsSystem.GetBodiesAt(x, y);
    }
}