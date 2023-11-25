using Box2DSharp.Dynamics.Contacts;

namespace Platformer.Component
{
    public class GroundedComponent
    {
        public Contact Contact { get; }

        public GroundedComponent(Contact contact)
        {
            Contact = contact;
        }
    }
}
