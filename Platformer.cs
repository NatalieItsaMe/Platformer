using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Tiled;
using Box2DSharp.Dynamics;
using Platformer.Systems;
using Platformer.Component;
using System.Linq;
using System.Text.Json;
using MonoGame.Extended;
using Vector2 = System.Numerics.Vector2;
using World = MonoGame.Extended.Entities.World;
using MonoGame.Extended.Sprites;

namespace Platformer
{
    public class Platformer : Game
    {
        private GraphicsDeviceManager _graphics;
        internal RenderSystem _renderSystem;
        private PhysicsSystem _physicsSystem;
        private World _world;

        public Platformer()
        {
            Window.AllowUserResizing = true;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _physicsSystem = new PhysicsSystem();
            _renderSystem = new RenderSystem(GraphicsDevice);
            _world = new WorldBuilder()
                .AddSystem(_renderSystem)
                .AddSystem(new PlayerInputSystem(this))
                .AddSystem(_physicsSystem)
                .Build();

            _physicsSystem.SetContactListener(new GroundContactListener(_world));
#if DEBUG
            var debug = _world.CreateEntity();
            debug.Attach(new Transform2());
            debug.Attach(new DebugController());
            //debug.Attach(new CameraTarget() { Zoom = 6.0f });
#endif

            base.Initialize();
        }

        protected override void LoadContent()
        {
            TiledMap tiledMap = Content.Load<TiledMap>("snowyTree");

            foreach (var mapObject in tiledMap.ObjectLayers.SelectMany(l => l.Objects))
            {
                Entity entity = _world.CreateEntity();
                Body body = _physicsSystem.CreateBodyFromTiledObject(mapObject, tiledMap.GetScale());
                body.UserData = entity.Id;
                entity.Attach(body);
                if(mapObject is TiledMapTileObject tileObject)
                {
                    int col = tileObject.Tile.LocalTileIdentifier % tileObject.Tileset.Columns;
                    int row = tileObject.Tile.LocalTileIdentifier / tileObject.Tileset.Columns;
                    var region = tileObject.Tileset.GetRegion(col, row);
                    entity.Attach(new Sprite(region));
                }

                if (mapObject.Properties.ContainsKey("CameraTarget"))
                {
                    var cameraTarget = JsonSerializer.Deserialize<CameraTarget>(mapObject.Properties["CameraTarget"]);
                    entity.Attach(cameraTarget);
                }
                if (mapObject.Properties.ContainsKey("KeyboardMapping"))
                {
                    var keyboardMapping = JsonSerializer.Deserialize<KeyboardMapping>(mapObject.Properties["KeyboardMapping"]);
                    entity.Attach(keyboardMapping);
                }
            }

            _renderSystem.SetTiledMap(tiledMap);
        }

        protected override void Update(GameTime gameTime)
        {
            _world.Update(gameTime);

            base.Update(gameTime);
        }

        internal Vector2 GetWorldCoordinates(float x, float y) => _renderSystem.GetCamera().ScreenToWorld(x, y).ToNumerics();

        internal Body[] GetBodiesAt(float x, float y) => _physicsSystem.GetBodiesAt(x, y);

        protected override void Draw(GameTime gameTime)
        {
            _world.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}