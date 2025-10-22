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
        private World _world;

        private OrthographicCamera _camera;
        private MyTiledMapRenderer _tiledRenderer;
        private DebugView _debugView;
        private SpriteBatch _worldSpriteBatch;
        private SpriteBatch _uiSpriteBatch;

        private Matrix _projectionMatrix;
        private Matrix _scaleMatrix;
        private Matrix _reflectionMatrix = Matrix.CreateReflection(new Plane(0f, 1f, 0f, 0f));


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
            _projectionMatrix = Matrix.CreateOrthographic(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0.0f, 1.0f);
            _worldSpriteBatch = new SpriteBatch(GraphicsDevice);
            _uiSpriteBatch = new SpriteBatch(GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            const string mapName = "MoveAndJump";
            TiledMap tiledMap = Content.Load<TiledMap>(mapName);
            _scaleMatrix = Matrix.CreateScale(tiledMap.TileWidth, tiledMap.TileHeight, 1.0f);
            _tiledRenderer = new MyTiledMapRenderer(GraphicsDevice, tiledMap);

            var physicsSystem = new PhysicsSystem();

            _world = new WorldBuilder()
                .AddSystem(new SpriteDrawSystem(_worldSpriteBatch))
                .AddSystem(new PlayerInputSystem(this))
                .AddSystem(physicsSystem)
                .AddSystem(new CameraTargetSystem(_camera) { WorldBounds = new(0, 0, tiledMap.WidthInPixels, tiledMap.HeightInPixels), ScaleMatrix = _scaleMatrix})
                .AddSystem(new DebugControllerSystem(this) { Camera = _camera, PhysicsSystem = physicsSystem })
                .Build();

            physicsSystem.RegisterContactListener(new OneWayContactListener(_world));
            physicsSystem.RegisterContactListener(new GroundedContactListener(_world));
            physicsSystem.RegisterContactListener(new SpringContactListener(_world));

            _debugView = new DebugView(physicsSystem);
            _debugView.AppendFlags(DebugViewFlags.Shape | DebugViewFlags.ContactPoints | DebugViewFlags.ContactNormals);
            _debugView.LoadContent(GraphicsDevice, Content);

            var entityFactory = new EntityComponentsFactory(_world, physicsSystem, this);
            entityFactory.BuildEntitiesFromTiledMap(tiledMap);
        }

        protected override void Update(GameTime gameTime)
        {
            _tiledRenderer.Update(gameTime);
            _world.Update(gameTime);
            _debugView.UpdatePerformanceGraph(gameTime.ElapsedGameTime);            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;

            //draw tiled background
            _tiledRenderer.DrawBackgroundLayers(_camera.GetViewMatrix());

            _worldSpriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
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
    }
}