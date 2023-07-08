using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Box2DSharp.Dynamics;
using Platformer.Components;
using System.Linq;

namespace Platformer
{
    internal class PhysicsSystem : EntityUpdateSystem
    {
        public System.Numerics.Vector2 Gravity = new System.Numerics.Vector2(0, 21f);
        private ComponentMapper<GroundedComponent> _grounded;
        private readonly Box2DSharp.Dynamics.World Box2DWorld;

        public PhysicsSystem() : base(Aspect.All(typeof(Body)))
        {
            Box2DWorld = new Box2DSharp.Dynamics.World(Gravity);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _grounded = mapperService.GetMapper<GroundedComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                if (_grounded.Has(entity) && Box2DWorld.BodyList.Single(body => (int)body.UserData == entity).IsAwake)
                    _grounded.Delete(entity);
            }

            Box2DWorld.Step(gameTime.GetElapsedSeconds(), 6, 2);
        }

        public Body CreateBody(BodyDef bodyDef) => Box2DWorld.CreateBody(bodyDef);

        public void SetContactListener(IContactListener listener) => Box2DWorld.SetContactListener(listener);
    }
}
