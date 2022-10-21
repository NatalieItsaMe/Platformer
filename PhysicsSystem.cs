using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace Platformer
{
    internal class PhysicsSystem : EntityUpdateSystem
    {
        public Vector2 Gravity = new Vector2(0, 0);

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
                var position = _transforms.Get(entity).Position;
                var velocity = _physics.Get(entity).Velocity;
                var acceleration = _physics.Get(entity).Acceleration;

                velocity += acceleration + Gravity;
                position += velocity;

                _physics.Get(entity).Velocity = velocity;
                _transforms.Get(entity).Position = position;
            }
        }
    }
}
