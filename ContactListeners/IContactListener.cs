using nkast.Aether.Physics2D.Collision;
using nkast.Aether.Physics2D.Dynamics.Contacts;

namespace Platformer.ContactListeners
{
    internal interface IContactListener
    {
        public abstract bool BeginContact(Contact contact);
        public abstract void EndContact(Contact contact);
        public abstract void PreSolve(Contact contact, ref Manifold oldManifold);
    }
}
