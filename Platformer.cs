using Box2DSharp.Dynamics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Content;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Serialization.Json;
using MonoGame.Extended.Tiled;
using Newtonsoft.Json;
using Platformer.Component;
using Platformer.Factories;
using Platformer.Systems;
using System.Linq;
using World = MonoGame.Extended.ECS.World;
namespace Platformer
{
    public class Platformer : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        internal TiledMapRenderSystem _renderSystem;
        private PhysicsSystem _physicsSystem;
        private World _world;

        public Platformer()
        {
            Window.AllowUserResizing = true;
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 960;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _physicsSystem = new PhysicsSystem();
            _renderSystem = new TiledMapRenderSystem();
            Box2dContactListener contactSystem = new Box2dContactListener();
            _world = new WorldBuilder()
                .AddSystem(_renderSystem)
                .AddSystem(new PlayerInputSystem(this))
                .AddSystem(_physicsSystem)
                .AddSystem(contactSystem)
                .Build();

            _physicsSystem.SetContactListener(contactSystem);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            const string mapName = "snowyTree";
            TiledMap tiledMap = Content.Load<TiledMap>(mapName);
            TiledBodyFactory bodyFactory = new(_physicsSystem.Box2DWorld, tiledMap);

            foreach (var mapObject in tiledMap.ObjectLayers.SelectMany(l => l.Objects))
            {
                Entity entity = _world.CreateEntity();
                Body body = bodyFactory.CreateBodyFromTiledObject(mapObject);
                body.UserData = entity.Id;
                entity.Attach(body);
                if(mapObject is TiledMapTileObject tileObject && tileObject.Tile != null)
                {
                    entity.Attach(tileObject);
                }
                if (mapObject.Properties.ContainsKey("CameraTarget"))
                {
                    var cameraTarget = JsonConvert.DeserializeObject<CameraTarget>(mapObject.Properties["CameraTarget"]);
                    entity.Attach(cameraTarget);
                }
                if (mapObject.Properties.ContainsKey("KeyboardMapping"))
                {
                    var keyboardMapping = JsonConvert.DeserializeObject<KeyboardController>(mapObject.Properties["KeyboardMapping"]);
                    entity.Attach(keyboardMapping);
                }
                if (mapObject.Properties.ContainsKey("OneWayPlatform"))
                {
                    var oneWay = JsonConvert.DeserializeObject<OneWayPlatform>(mapObject.Properties["OneWayPlatform"]);
                    entity.Attach(oneWay);
                }
                if (mapObject.Properties.ContainsKey("SpringComponent"))
                {
                    var spring = JsonConvert.DeserializeObject<SpringComponent>(mapObject.Properties["SpringComponent"]);
                    entity.Attach(spring);
                }
                if (mapObject.Properties.ContainsKey("SpriteSheet"))
                {
                    var spriteSheet = Content.Load<SpriteSheet>(mapObject.Properties["SpriteSheet"], new JsonContentLoader());

                    mapObject.Properties.TryGetValue("PlayAnimation", out string playAnimation);
                    var sprite = new AnimatedSprite(spriteSheet, playAnimation);
                    sprite.Origin = Vector2.UnitY * 18f + mapObject.Size.ToNumerics() / new System.Numerics.Vector2(2f, -2f);
                    entity.Attach(sprite);
                }
            }

            _renderSystem.SetTiledMap(GraphicsDevice, tiledMap);
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

        internal Body[] GetBodiesAt(float x, float y) => _physicsSystem.GetBodiesAt(x, y);
    }
}