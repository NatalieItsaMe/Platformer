using Box2DSharp.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Platformer.Components;
using System;

namespace Platformer
{
    internal class PlayerInputSystem : EntityUpdateSystem
    {
        private const float HorizontalMovementForce = 180.0f;
        private const float MaxHorizontalSpeed = 16f;
        private const float JumpForce = -800.0f;
        private Platformer _game;
        private ComponentMapper<KeyboardMapping> _keyboardMappings;
        private ComponentMapper<Body> _bodies;

        public PlayerInputSystem() : base(Aspect.All(typeof(Body)).One(typeof(KeyboardMapping))) { }

        public PlayerInputSystem(Platformer game) : base(Aspect.All(typeof(Transform2), typeof(Body)).One(typeof(KeyboardMapping)))
        {
            _game = game;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _keyboardMappings = mapperService.GetMapper<KeyboardMapping>();
            _bodies = mapperService.GetMapper<Body>();
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

                    if (state.IsKeyDown(mapping.Up))
                        body.ApplyForceToCenter(new(0, JumpForce), true);
                    if (state.IsKeyDown(mapping.Right))
                        body.ApplyForceToCenter(new(HorizontalMovementForce,0), true);
                    if (state.IsKeyDown(mapping.Left))
                        body.ApplyForceToCenter(new(-HorizontalMovementForce,0), true);

                    if(Math.Abs(body.LinearVelocity.X) > MaxHorizontalSpeed)
                    {
                        body.SetLinearVelocity(new (Math.Sign(body.LinearVelocity.X) * MaxHorizontalSpeed, body.LinearVelocity.Y));
                    }

                    if((body.LinearVelocity.X > 0 && state.IsKeyUp(mapping.Right)) || (body.LinearVelocity.X < 0) && state.IsKeyUp(mapping.Left))
                    {
                        body.AngularDamping = float.MaxValue;
                    }
                    else
                    {
                        body.AngularDamping = 0;
                    }
                }

#if DEBUG
                _game._renderSystem.Messages.Add(new($"           Body: {body}", Color.Black));
                _game._renderSystem.Messages.Add(new($"AngularVelocity: {body.AngularVelocity}", Color.Black));
                _game._renderSystem.Messages.Add(new($" LinearVelocity: {body.LinearVelocity}", Color.Black));
                _game._renderSystem.Messages.Add(new($"        Inertia: {body.Inertia}", Color.Black));
#endif
            }
        }
    }
}
