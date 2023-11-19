using Box2DSharp.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Platformer.Component;
using System;
using System.Linq;

namespace Platformer
{
    internal class PlayerInputSystem : EntityUpdateSystem
    {
        private const float HorizontalMovementForce = 32f;
        private const float MaxHorizontalSpeed = 4f;
        private const float JumpForce = -200;
        private const float MaxAngularDamping = 80;
        private Platformer _game;
        private ComponentMapper<KeyboardMapping> _keyboardMappings;
        private ComponentMapper<Body> _bodies;
        private ComponentMapper<GroundedComponent> _grounded;
        private ComponentMapper<DebugController> _debug;
        private ComponentMapper<Transform2> _transform;
        private ComponentMapper<CameraTarget> _cameraTarget;

        public PlayerInputSystem(Platformer game) : base(Aspect.One(typeof(Body), typeof(KeyboardMapping), typeof(DebugController)))
        {
            _game = game;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _keyboardMappings = mapperService.GetMapper<KeyboardMapping>();
            _bodies = mapperService.GetMapper<Body>();
            _grounded = mapperService.GetMapper<GroundedComponent>();
            _debug = mapperService.GetMapper<DebugController>();
            _cameraTarget = mapperService.GetMapper<CameraTarget>();
            _transform = mapperService.GetMapper<Transform2>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities.Where(e => _bodies.Has(e) && _keyboardMappings.Has(e)))
                UpdatePlayerEntity(entity);

            foreach (var entity in ActiveEntities.Where(e => _transform.Has(e) && _debug.Has(e)))
                UpdateDebugEntity(entity);
        }

        private void UpdateDebugEntity(int entity)
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();
            Transform2 transform = _transform.Get(entity);
            CameraTarget cameraTarget = _cameraTarget.Get(entity);

            if (keyboard.IsKeyDown(Keys.Escape))
                _game.Exit();

            if (keyboard.IsKeyDown(Keys.Up))
                transform.Position = transform.Position.Translate(0, -1);
            if (keyboard.IsKeyDown(Keys.Down))
                transform.Position = transform.Position.Translate(0, 1);
            if (keyboard.IsKeyDown(Keys.Right))
                transform.Position = transform.Position.Translate(1, 0);
            if (keyboard.IsKeyDown(Keys.Left))
                transform.Position = transform.Position.Translate(-1, 0);

            if (keyboard.IsKeyDown(Keys.PageDown))
                cameraTarget.Zoom *= 0.9f;            
            if (keyboard.IsKeyDown(Keys.PageUp))
                cameraTarget.Zoom *= 1.1f;

            Vector2 worldMouse = _game.GetWorldCoordinates(mouse.X, mouse.Y);
            var bodiesUnderMouse = _game.GetBodiesAt(worldMouse.X, worldMouse.Y);

            foreach(var body in bodiesUnderMouse)
            {
                System.Diagnostics.Debug.WriteLine($"-----------body: {body.UserData}");
            }

            if(mouse.LeftButton == ButtonState.Pressed)
            {
                foreach(var body in bodiesUnderMouse)
                {
                    System.Diagnostics.Debug.WriteLine($"       Position: {body.GetPosition()}");
                    System.Diagnostics.Debug.WriteLine($"      IsEnabled: {body.IsEnabled}");
                    System.Diagnostics.Debug.WriteLine($"        IsAwake: {body.IsAwake}");
                    System.Diagnostics.Debug.WriteLine($"           Mass: {body.Mass}");
                    System.Diagnostics.Debug.WriteLine($" LinearVelocity: {body.LinearVelocity}");
                    System.Diagnostics.Debug.WriteLine($"AngularVelocity: {body.AngularVelocity}");
                    System.Diagnostics.Debug.WriteLine($"-------Fixtures: {body.FixtureList.Count}");

                    foreach(var fixture in body.FixtureList)
                    {
                        System.Diagnostics.Debug.WriteLine($"        Fixture: {fixture.ShapeType}");
                        System.Diagnostics.Debug.WriteLine($"        Density: {fixture.Density}");
                        System.Diagnostics.Debug.WriteLine($"       Friction: {fixture.Friction}");
                        System.Diagnostics.Debug.WriteLine($"    Restitution: {fixture.Restitution}");
                    }
                }
            }
        }

        private void UpdatePlayerEntity(int entity)
        {
            Body body = _bodies.Get(entity);
            var mapping = _keyboardMappings.Get(entity);
            var state = Keyboard.GetState();

            if (state.IsKeyDown(mapping.Exit))
                _game.Exit();

            if (_grounded.Has(entity))
            {
                if (state.IsKeyDown(mapping.Up))
                    body.ApplyForceToCenter(new(0, JumpForce), true);
                if (state.IsKeyDown(mapping.Right))
                    body.ApplyForceToCenter(new(HorizontalMovementForce, 0), true);
                if (state.IsKeyDown(mapping.Left))
                    body.ApplyForceToCenter(new(-HorizontalMovementForce, 0), true);

                if ((body.LinearVelocity.X > 0 && state.IsKeyUp(mapping.Right)) || (body.LinearVelocity.X < 0) && state.IsKeyUp(mapping.Left))
                {
                    body.AngularDamping = MaxAngularDamping;
                }
                else
                {
                    body.AngularDamping = 0;
                }
            }

            if (Math.Abs(body.LinearVelocity.X) > MaxHorizontalSpeed)
            {
                body.SetLinearVelocity(new(Math.Sign(body.LinearVelocity.X) * MaxHorizontalSpeed, body.LinearVelocity.Y));
            }
        }
    }
}
