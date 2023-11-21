using Box2DSharp.Collision.Collider;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using Platformer.Component;
using System.Drawing;

namespace Platformer
{
    internal class GroundContactListener : IContactListener
    {
        private readonly MonoGame.Extended.Entities.World _world;
        private WorldManifold worldManifold;

        public GroundContactListener(MonoGame.Extended.Entities.World world)
        {
            _world = world;
        }

        public void BeginContact(Contact contact) { }

        public void EndContact(Contact contact) { }

        public void PreSolve(Contact contact, in Manifold oldManifold)
        {
            contact.GetWorldManifold(out worldManifold);
            Entity entityA = _world.GetEntity((int)contact.FixtureA.Body.UserData);
            Entity entityB = _world.GetEntity((int)contact.FixtureB.Body.UserData);

            if (IsFixtureGrounded(contact.FixtureA))
                entityA.Attach(new GroundedComponent());
            if (IsFixtureGrounded(contact.FixtureB))
                entityB.Attach(new GroundedComponent());
        }

        public void PostSolve(Contact contact, in ContactImpulse impulse) { }

        private bool IsFixtureGrounded(Fixture fixture) => fixture.Body.GetWorldCenter().Y < worldManifold.Points.Value0.Y;
    }
}
