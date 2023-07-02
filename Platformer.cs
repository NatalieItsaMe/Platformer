using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;
using Platformer.Components;
using Platformer.Systems;
using System;
using World = MonoGame.Extended.Entities.World;

namespace Platformer
{
    public class Platformer : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private World _world;

        private Entity _ball;
        private Entity _ground;

        public Platformer()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            PhysicsSystem physicsSystem = new PhysicsSystem();
            _world = new WorldBuilder()
                .AddSystem(new RenderSystem(GraphicsDevice))
                .AddSystem(new PlayerInputSystem(this))
                .AddSystem(physicsSystem)
                .Build();

            InitializeBall(physicsSystem);
            InitializeGround(physicsSystem);

            base.Initialize();
        }

        private void InitializeBall(PhysicsSystem physicsSystem)
        {
            _ball = _world.CreateEntity();

            BodyDef bodyDef = new BodyDef()
            {
                Position = new System.Numerics.Vector2(100, 100),
                BodyType = BodyType.DynamicBody
            };
            Body body = physicsSystem.CreateBody(bodyDef);
            CircleShape shape = new CircleShape()
            {
                Radius = 10
            };
            FixtureDef fixture = new FixtureDef()
            {
                Shape = shape,
                Density = 1.0f,
                Friction = 0.3f
            };
            body.CreateFixture(fixture);

            _ball.Attach(body);
        }

        private void InitializeGround(PhysicsSystem physicsSystem)
        {
            _ground = _world.CreateEntity();

            BodyDef bodyDef = new BodyDef()
            {
                Position = new System.Numerics.Vector2(100, 120)                
            };
            Body body = physicsSystem.CreateBody(bodyDef);
            PolygonShape shape = new PolygonShape();
            shape.SetAsBox(16, 2);
            body.CreateFixture(shape, 0.0f);

            _ground.Attach(body);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _ball.Attach(new Sprite(Content.Load<Texture2D>("ball")));
            _ball.Attach(new Transform2());
            _ball.Attach(new KeyboardMapping());

            _ground.Attach(new Sprite(Content.Load<Texture2D>("ground")));
            _ground.Attach(new Transform2());
            
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