using nkast.Aether.Physics2D.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Platformer.Component;
using System;

namespace Platformer.Systems
{
    internal class PlayerInputSystem(PlatformerGame game) : EntityUpdateSystem(Aspect.All(typeof(Body)).One(typeof(KeyboardController)))
    {
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
            var controller = _keyboards.Get(entity);
            var state = Keyboard.GetState();

            if (state.IsKeyDown(controller.Exit))
                _game.Exit();

            if (_grounded.Has(entity))
            {
                if(JumpTimeout > 0) 
                    JumpTimeout--;

                if (state.IsKeyDown(controller.Jump) && JumpTimeout == 0)
                {
                    _grounded.Delete(entity);
                    body.ApplyForce(new(0, controller.JumpForce));
                    JumpTimeout = controller.MaxJumpTimeout;
                }

                if (state.IsKeyDown(controller.Right))
                    body.ApplyForce(new(controller.HorizontalMovementForce, 0));
                if (state.IsKeyDown(controller.Left))
                    body.ApplyForce(new(-controller.HorizontalMovementForce, 0));
            }

            if (Math.Abs(body.LinearVelocity.X) > controller.MaxHorizontalSpeed)
            {
                body.LinearVelocity.SetX(Math.Sign(body.LinearVelocity.X) * controller.MaxHorizontalSpeed);
            }
        }
    }
}
