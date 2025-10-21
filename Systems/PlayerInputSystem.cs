using nkast.Aether.Physics2D.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Platformer.Component;
using System;
using System.Linq;

namespace Platformer.Systems
{
    internal class PlayerInputSystem(PlatformerGame game) : EntityUpdateSystem(Aspect.All(typeof(Body), typeof(KeyboardController)))
    {
        private const float HorizontalMovementForce = 36f;
        private const float MaxHorizontalSpeed = 4.2f;
        private const float JumpForce = -360f;
        private const ushort MaxJumpTimeout = 4;
        private const ushort MaxZoomTimeout = 6;

        private readonly PlatformerGame _game = game;
        private ComponentMapper<KeyboardController> _keyboards;
        private ComponentMapper<Body> _bodies;
        private ComponentMapper<GroundedComponent> _grounded;
        private ushort JumpTimeout;

        public override void Initialize(IComponentMapperService mapperService)
        {
            _keyboards = mapperService.GetMapper<KeyboardController>();
            _bodies = mapperService.GetMapper<Body>();
            _grounded = mapperService.GetMapper<GroundedComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
                UpdatePlayerEntity(entity);
        }

        private void UpdatePlayerEntity(int entity)
        {
            Body body = _bodies.Get(entity);
            var mapping = _keyboards.Get(entity);
            var state = Keyboard.GetState();

            if (state.IsKeyDown(mapping.Exit))
                _game.Exit();

            if (_grounded.Has(entity))
            {
                if(JumpTimeout > 0) 
                    JumpTimeout--;

                if (state.IsKeyDown(mapping.Jump) && JumpTimeout == 0)
                {
                    _grounded.Delete(entity);
                    body.ApplyForce(new(0, JumpForce));
                    JumpTimeout = MaxJumpTimeout;
                }

                if (state.IsKeyDown(mapping.Right))
                    body.ApplyForce(new(HorizontalMovementForce, 0));
                if (state.IsKeyDown(mapping.Left))
                    body.ApplyForce(new(-HorizontalMovementForce, 0));
            }

            if (Math.Abs(body.LinearVelocity.X) > MaxHorizontalSpeed)
            {
                body.LinearVelocity.SetX(Math.Sign(body.LinearVelocity.X) * MaxHorizontalSpeed);
            }
        }
    }
}
