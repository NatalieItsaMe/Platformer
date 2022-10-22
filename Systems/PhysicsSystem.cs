using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Platformer.Components;

namespace Platformer
{
    internal class PhysicsSystem : EntityUpdateSystem
    {
        public Vector2 Gravity = new Vector2(0, 0);
        public Vector2 TerminalVelocity = new Vector2(12.4f, 12.4f);

        private ComponentMapper<Transform2> _transforms;
        private ComponentMapper<Physics> _physics;

        public PhysicsSystem() : base(Aspect.All(typeof(Physics), typeof(Transform2)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transforms = mapperService.GetMapper<Transform2>();
            _physics = mapperService.GetMapper<Physics>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach(var entity in ActiveEntities)
            {
                UpdateVelocity(gameTime.GetElapsedSeconds(), entity);
                UpdatePosition(gameTime.GetElapsedSeconds(), entity);
            }
        }

        private void UpdatePosition(float seconds, int entity)
        {
            var velocity = _physics.Get(entity).Velocity;

            _transforms.Get(entity).Position += velocity * seconds;
        }

        private void UpdateVelocity(float seconds, int entity)
        {
            var velocity = _physics.Get(entity).Velocity;
            var acceleration = _physics.Get(entity).Acceleration;

            velocity += acceleration * seconds;
            velocity += Gravity * seconds;
            if (velocity.Y > TerminalVelocity.Y)
                velocity.SetY(TerminalVelocity.Y);
            if (velocity.X > TerminalVelocity.X)
                velocity.SetX(TerminalVelocity.X);

            _physics.Get(entity).Velocity = velocity;
        }
    }
}
