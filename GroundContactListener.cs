using Box2DSharp.Collision.Collider;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using MonoGame.Extended.Entities;
using Platformer.Component;

namespace Platformer
{
    internal class GroundContactListener : IContactListener
    {
        private const float GroundNormal = 0.5f;
        private readonly MonoGame.Extended.Entities.World _world;
        private WorldManifold worldManifold;

        public GroundContactListener(MonoGame.Extended.Entities.World world)
        {
            _world = world;
        }

        public void BeginContact(Contact contact)
        {
            contact.GetWorldManifold(out worldManifold);

            //fixure A is the start, -normal points towards A
            if (worldManifold.Normal.Y > GroundNormal)
            {
                Entity entityA = _world.GetEntity((int)contact.FixtureA.Body.UserData);
                entityA.Attach(new GroundedComponent(contact));
            }
            //fixure B is the end, normal points towards B
            if (-worldManifold.Normal.Y > GroundNormal)
            {
                Entity entityB = _world.GetEntity((int)contact.FixtureB.Body.UserData);
                entityB.Attach(new GroundedComponent(contact));
            }
        }

        public void EndContact(Contact contact)
        {
            Entity entityA = _world.GetEntity((int)contact.FixtureA.Body.UserData);
            Entity entityB = _world.GetEntity((int)contact.FixtureB.Body.UserData);

            //fixure A is the start, -normal points towards A
            if (entityA.Has<GroundedComponent>() && entityA.Get<GroundedComponent>().Contact == contact)
            {
                entityA.Detach<GroundedComponent>();
                System.Diagnostics.Debug.WriteLine($" ungrounded:{entityA.Id})");
            }
            //fixure B is the end, normal points towards B
            if (entityB.Has<GroundedComponent>() && entityB.Get<GroundedComponent>().Contact == contact)
            {
                entityB.Detach<GroundedComponent>();
                System.Diagnostics.Debug.WriteLine($" ungrounded:{entityB.Id})");
            }
        }

        public void PreSolve(Contact contact, in Manifold oldManifold) { }

        public void PostSolve(Contact contact, in ContactImpulse impulse) { }
    }
}
