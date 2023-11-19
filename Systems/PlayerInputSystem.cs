using Box2DSharp.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Platformer.Component;
using System;

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

        public PlayerInputSystem() : base(Aspect.All(typeof(Body)).One(typeof(KeyboardMapping))) { }

        public PlayerInputSystem(Platformer game) : base(Aspect.All(typeof(Body), typeof(KeyboardMapping)))
        {
            _game = game;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _keyboardMappings = mapperService.GetMapper<KeyboardMapping>();
            _bodies = mapperService.GetMapper<Body>();
            _grounded = mapperService.GetMapper<GroundedComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach(var entity in ActiveEntities)
            {
                Body body = _bodies.Get(entity);
                if(_keyboardMappings.Has(entity))
                {
                    var state = Keyboard.GetState();
                    var mapping = _keyboardMappings.Get(entity);

                    if (state.IsKeyDown(mapping.Exit))
                        _game.Exit();

                    if (_grounded.Has(entity))
                    {
                        if (state.IsKeyDown(mapping.Up))
                            body.ApplyForceToCenter(new(0, JumpForce), true);
                        if (state.IsKeyDown(mapping.Right))
                            body.ApplyForceToCenter(new(HorizontalMovementForce,0), true);
                        if (state.IsKeyDown(mapping.Left))
                            body.ApplyForceToCenter(new(-HorizontalMovementForce,0), true);

                        if((body.LinearVelocity.X > 0 && state.IsKeyUp(mapping.Right)) || (body.LinearVelocity.X < 0) && state.IsKeyUp(mapping.Left))
                        {
                            body.AngularDamping = MaxAngularDamping;
                        }
                        else
                        {
                            body.AngularDamping = 0;
                        }
                    }

                    if(Math.Abs(body.LinearVelocity.X) > MaxHorizontalSpeed)
                    {
                        body.SetLinearVelocity(new (Math.Sign(body.LinearVelocity.X) * MaxHorizontalSpeed, body.LinearVelocity.Y));
                    }
                }
            }
        }
    }
}
