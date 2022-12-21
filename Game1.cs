using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;

namespace Platformer
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private World _world;

        private Entity _ball, _wall;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _world = new WorldBuilder()
                .AddSystem(new RenderSystem(GraphicsDevice))
                .Build();

            _ball = _world.CreateEntity();
            _wall = _world.CreateEntity();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _ball.Attach(new Sprite(Content.Load<Texture2D>("ball")));
            _ball.Attach(new Transform2(100, 100));
            _ball.Attach(new Body()
            {
                Rectangle = new RectangleF(-24, -24, 48, 48),
                Type = Body.BodyType.Kinematic
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
            float speed = 26.0f;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.W))
                _ball.Get<Transform2>().Position -= Vector2.UnitY * speed * gameTime.GetElapsedSeconds();
            if (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.S))
                _ball.Get<Transform2>().Position += Vector2.UnitY * speed * gameTime.GetElapsedSeconds();
            if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.A))
                _ball.Get<Transform2>().Position -= Vector2.UnitX * speed * gameTime.GetElapsedSeconds();
            if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.D))
                _ball.Get<Transform2>().Position += Vector2.UnitX * speed * gameTime.GetElapsedSeconds();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _world.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}