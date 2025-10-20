using nkast.Aether.Physics2D.Dynamics.Contacts;

namespace Platformer.Component
{
    public class GroundedComponent(Contact contact)
    {
        public Contact Contact { get; } = contact;
    }
}
