using Microsoft.Xna.Framework;
using Box2DSharp.Dynamics;
using Platformer.Systems;
using Platformer.Component;
using System.Linq;
using System.Text.Json;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Entities;
using World = MonoGame.Extended.Entities.World;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using MonoGame.Extended.TextureAtlases;
using Platformer.Factories;

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
            _graphics.PreferredBackBufferWidth = 1080;
            _graphics.PreferredBackBufferHeight = 920;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _physicsSystem = new PhysicsSystem();
            _renderSystem = new TiledMapRenderSystem();
            _world = new WorldBuilder()
                .AddSystem(_renderSystem)
                .AddSystem(new PlayerInputSystem(this))
                .AddSystem(_physicsSystem)
                .Build();

            _physicsSystem.SetContactListener(new GroundContactListener(_world));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            const string mapName = "snowyTree";
            TiledMap tiledMap = Content.Load<TiledMap>(mapName);
            TiledBodyFactory bodyFactory = new(_physicsSystem.Box2DWorld, tiledMap);
            TiledSpriteFactory spriteFactory = new(tiledMap);

            foreach (var mapObject in tiledMap.ObjectLayers.SelectMany(l => l.Objects))
            {
                Entity entity = _world.CreateEntity();
                Body body = bodyFactory.CreateBodyFromTiledObject(mapObject);
                body.UserData = entity.Id;
                entity.Attach(body);
                if(mapObject is TiledMapTileObject tileObject && tileObject.Tile != null)
                {
                    if(tileObject.Tile is TiledMapTilesetAnimatedTile animatedTile)
                    {
                        AnimatedSprite sprite = spriteFactory.CreateAnimatedSprite(tileObject);
                        entity.Attach(sprite);
                    }
                    else
                    {
                        Sprite sprite = spriteFactory.CreateSprite(tileObject);
                        entity.Attach(sprite);
                    }
                }
                if (mapObject.Properties.ContainsKey("CameraTarget"))
                {
                    var cameraTarget = JsonSerializer.Deserialize<CameraTarget>(mapObject.Properties["CameraTarget"]);
                    entity.Attach(cameraTarget);
                }
                if (mapObject.Properties.ContainsKey("KeyboardMapping"))
                {
                    var keyboardMapping = JsonSerializer.Deserialize<KeyboardController>(mapObject.Properties["KeyboardMapping"]);
                    entity.Attach(keyboardMapping);
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