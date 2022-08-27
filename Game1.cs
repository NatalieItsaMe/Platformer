using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Sprites;

namespace Platformer
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private World _world;

        private Entity _ball;

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
                .AddSystem(new PlayerInputSystem())
                .Build();

            _ball = _world.CreateEntity();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _ball.Attach(new Sprite(Content.Load<Texture2D>("ball")));
            _ball.Attach(new Transform2());
            _ball.Attach(new KeyboardMapping());
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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