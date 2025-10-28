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
            {
                Body body = _bodies.Get(entity);
                var controller = _keyboards.Get(entity);
                var state = Keyboard.GetState();

                if (state.IsKeyDown(controller.Exit))
                    _game.Exit();

                bool isGrounded = _grounded.Has(entity);
                HorizontalMovement(body, controller, state);

                VerticalMovement(body, controller, state, ref isGrounded);
                if (!isGrounded)
                    _grounded.Delete(entity);
            }
        }

        private void VerticalMovement(Body body, KeyboardController controller, KeyboardState state, ref bool isGrounded)
        {
            if (state.IsKeyUp(controller.Jump) && body.LinearVelocity.Y < 0)
                body.LinearVelocity = Vector2.Lerp(body.LinearVelocity.SetY(0), body.LinearVelocity, 0.33f);

            if (!isGrounded)
                return;

            if (JumpTimeout > 0)
                JumpTimeout--;

            if (state.IsKeyDown(controller.Jump) && JumpTimeout == 0)
            {
                body.ApplyLinearImpulse(Vector2.UnitY * controller.JumpForce);
                JumpTimeout = controller.MaxJumpTimeout;
                isGrounded = false;
            }
        }

        private static void HorizontalMovement(Body body, KeyboardController controller, KeyboardState state)
        {
            if (state.IsKeyUp(controller.Right) && state.IsKeyUp(controller.Left))
                body.LinearVelocity = Vector2.Lerp(body.LinearVelocity.SetX(0), body.LinearVelocity, 0.5f);

            if (state.IsKeyDown(controller.Right))
                body.ApplyForce(Vector2.UnitX * controller.HorizontalMovementForce);
            if (state.IsKeyDown(controller.Left))
                body.ApplyForce(Vector2.UnitX * -controller.HorizontalMovementForce);

            if (Math.Abs(body.LinearVelocity.X) > controller.MaxHorizontalSpeed)
                body.LinearVelocity = body.LinearVelocity.SetX(Math.Sign(body.LinearVelocity.X) * controller.MaxHorizontalSpeed);
        }
    }

    public enum MovementState
    {
        Idle, 
        ControlledJump, 
        FreeJump, 
        WalkingRight,
        WalkingLeft
    }
}
