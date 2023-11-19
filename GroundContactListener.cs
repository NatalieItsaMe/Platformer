using Box2DSharp.Collision.Collider;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using MonoGame.Extended.Entities;
using Platformer.Component;

namespace Platformer
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
            Entity entityA = _world.GetEntity((int)contact.FixtureA.Body.UserData);
            Entity entityB = _world.GetEntity((int)contact.FixtureB.Body.UserData);
            if (contact.Manifold.LocalNormal.Y != 0)
            {
                if(contact.FixtureA.Body.BodyType == BodyType.KinematicBody || contact.FixtureA.Body.BodyType == BodyType.DynamicBody)
                    entityA.Attach(new GroundedComponent());
                if(contact.FixtureB.Body.BodyType == BodyType.KinematicBody || contact.FixtureB.Body.BodyType == BodyType.DynamicBody)
                    entityB.Attach(new GroundedComponent());
            }
        }

        public void PreSolve(Contact contact, in Manifold oldManifold) { }
    }
}
