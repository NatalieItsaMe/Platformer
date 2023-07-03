using Box2DSharp.Collision.Collider;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using MonoGame.Extended.Entities;

namespace Platformer.Components
{
    internal class GroundContactListener : IContactListener
    {
        private readonly MonoGame.Extended.Entities.World _world;

        public GroundContactListener(MonoGame.Extended.Entities.World world)
        {
            _world = world;
        }

        public void BeginContact(Contact contact) { }

        public void EndContact(Contact contact) { }

        public void PostSolve(Contact contact, in ContactImpulse impulse) 
        {
            //add grounded to the entity
            Entity entityA = _world.GetEntity((int)contact.FixtureA.Body.UserData);
            Entity entityB = _world.GetEntity((int)contact.FixtureB.Body.UserData);

            entityA.Attach(new GroundedComponent());
            entityB.Attach(new GroundedComponent());
        }

        public void PreSolve(Contact contact, in Manifold oldManifold) { }
    }
}
