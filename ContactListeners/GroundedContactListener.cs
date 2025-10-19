using nkast.Aether.Physics2D.Dynamics.Contacts;
using Platformer.Component;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Collision;

namespace Platformer.ContactListeners
{
    internal class GroundedContactListener(MonoGame.Extended.ECS.World world) : IContactListener
    {
        private const float GroundNormal = 0.5f;
        private Vector2 normal;

        public bool BeginContact(Contact contact)
        {
            contact.GetWorldManifold(out normal, out _);
            AttachGroundedComponents(contact);
            return true;
        }

        public void EndContact(Contact contact)
        {
            //reset the default state of the contact in case it comes back for more
            contact.Enabled = true;
            DetachGroundedComponents(contact, (int)contact.FixtureA.Body.Tag);
            DetachGroundedComponents(contact, (int)contact.FixtureB.Body.Tag);
        }

        public void PreSolve(Contact contact, ref Manifold oldManifold) { }

        private void AttachGroundedComponents(Contact contact)
        {
            //fixure A is the start, -normal points towards A
            if (normal.Y > GroundNormal)
            {
                world.GetEntity((int)contact.FixtureA.Body.Tag).Attach(new GroundedComponent(contact));
            }
            //fixure B is the end, normal points towards B
            if (-normal.Y > GroundNormal)
            {
                world.GetEntity((int)contact.FixtureB.Body.Tag).Attach(new GroundedComponent(contact));
            }
        }

        private void DetachGroundedComponents(Contact contact, int entityID)
        {
            var entity = world.GetEntity(entityID);
            if (!entity.Has<GroundedComponent>())
                return;

            var grounded = entity.Get<GroundedComponent>();
            if (grounded.Contact != contact)
                return;

            entity.Detach<GroundedComponent>();
        }
    }
}
