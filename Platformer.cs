using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using Platformer.Component;
using Platformer.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using World = MonoGame.Extended.Entities.World;

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            TiledMap tiledMap = Content.Load<TiledMap>("sandbox");

            foreach (var mapObject in tiledMap.ObjectLayers.SelectMany(l => l.Objects))
            {
                Entity entity = _world.CreateEntity();
                Body body = _physicsSystem.AddTiledMapObject(mapObject);
                body.UserData = entity.Id;
                entity.Attach(body);

                foreach(var property in mapObject.Properties)
                {
                    var type = Assembly.Load("Platformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null").GetType("Platformer.Component." + property.Key, false, true);

                    if (type == null) continue;

                    var component = Activator.CreateInstance(type);
                    entity.Attach(component);
                }
            }

            TiledMapRenderer mapRenderer = new(GraphicsDevice, tiledMap);
            Entity map = _world.CreateEntity();

            map.Attach(mapRenderer);
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
    }
}