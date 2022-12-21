using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;
using System;
using System.Diagnostics;

namespace Platformer
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private World _world;

        private RenderSystem _renderSystem;

        private Entity _ball, _wall;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _renderSystem = new RenderSystem(GraphicsDevice);

            _world = new WorldBuilder()
                .AddSystem(new CollisionSystem())
                .AddSystem(new MovementSystem())
                .AddSystem(_renderSystem)
                .Build();

            _ball = _world.CreateEntity();
            _wall = _world.CreateEntity();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _renderSystem.DebugFont = Content.Load<SpriteFont>("Hud");

            // TODO: use this.Content to load your game content here
            _ball.Attach(new Sprite(Content.Load<Texture2D>("ball")));
            _ball.Attach(new Transform2(100, 100));
            _ball.Attach(new Body()
            {
                Rectangle = new RectangleF(-24, -24, 48, 48),
                Type = Body.BodyType.Kinematic,
                MaxSpeed = new Vector2(165, 165)
            });

            _wall.Attach(new Transform2(250, 100));
            _wall.Attach(new Body()
            {
                Rectangle = new RectangleF(-24, -64, 48, 128),
                Type = Body.BodyType.Static
            });
            
        }

        protected override void Update(GameTime gameTime)
        {
            InputUpdate(gameTime);

            _world.Update(gameTime);

            base.Update(gameTime);
        }

        private void InputUpdate(GameTime gameTime)
        {
            float force = 825f;
            float friction = 0.2f;
            Body body = _ball.Get<Body>();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.W))
                body.Acceleration -= Vector2.UnitY * force * gameTime.GetElapsedSeconds();
            if (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.S))
                body.Acceleration += Vector2.UnitY * force * gameTime.GetElapsedSeconds();
            if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.A))
                body.Acceleration -= Vector2.UnitX * force * gameTime.GetElapsedSeconds();
            if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.D))
                body.Acceleration += Vector2.UnitX * force * gameTime.GetElapsedSeconds();

            if (Keyboard.GetState().IsKeyUp(Keys.W) && body.Velocity.Y < 0)
            {
                if(body.Acceleration.Y < 0)
                    body.Acceleration = body.Acceleration.SetY(0);
                body.Velocity = body.Velocity.SetY(MathHelper.Lerp(body.Velocity.Y, 0, friction));
            }
            if (Keyboard.GetState().IsKeyUp(Keys.S) && body.Velocity.Y > 0)
            {
                if (body.Acceleration.Y > 0)
                    body.Acceleration = body.Acceleration.SetY(0);
                body.Velocity = body.Velocity.SetY(MathHelper.Lerp(body.Velocity.Y, 0, friction));
            }
            if (Keyboard.GetState().IsKeyUp(Keys.A) && body.Velocity.X < 0)
            {
                if (body.Acceleration.X < 0)
                    body.Acceleration = body.Acceleration.SetX(0);
                body.Velocity = body.Velocity.SetX(MathHelper.Lerp(body.Velocity.X, 0, friction));
            }
            if (Keyboard.GetState().IsKeyUp(Keys.D) && body.Velocity.X > 0)
            {
                if (body.Acceleration.X > 0)
                    body.Acceleration = body.Acceleration.SetX(0);
                body.Velocity = body.Velocity.SetX(MathHelper.Lerp(body.Velocity.X, 0, friction));
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _world.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}