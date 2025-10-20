using nkast.Aether.Physics2D.Collision;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using Platformer.Component;
using System.Collections.Generic;

namespace Platformer.ContactListeners
{
    internal class OneWayContactListener(MonoGame.Extended.ECS.World world) : IContactListener
    {
        private readonly List<Contact> DisabledContacts = [];
        //private FixedArray2<Vector2> points;

        public bool BeginContact(Contact contact)
        {
            var entityA = world.GetEntity((int)contact.FixtureA.Body.Tag);
            var entityB = world.GetEntity((int)contact.FixtureB.Body.Tag);
            if ((entityA.Has<OneWayPlatform>() && !IsContactSolid(contact, entityA.Get<OneWayPlatform>(), contact.FixtureB))
                || (entityB.Has<OneWayPlatform>() && !IsContactSolid(contact, entityB.Get<OneWayPlatform>(), contact.FixtureA)))
            {
                DisabledContacts.Add(contact);
                contact.Enabled = false;
            }

            return true;
        }

        public void EndContact(Contact contact) 
        {
            DisabledContacts.Remove(contact);
            contact.Enabled = true;
        }

        public void PreSolve(Contact contact, ref Manifold oldManifold)
        {
            if(DisabledContacts.Contains(contact))
                contact.Enabled = false;
        }

        private static bool IsContactSolid(Contact contact, OneWayPlatform oneWay, Fixture otherFixture)
        {
            contact.GetWorldManifold(out _, out var points);
            //no points are moving into the one-way's blocking direction, then contact should not be solid
            for (int i = 0; i < contact.Manifold.PointCount; i++)
            {
                var pointVel = otherFixture.Body.GetLinearVelocityFromWorldPoint(points[i]);
                switch (oneWay.Direction)
                {
                    case OneWayPlatform.PlatformDirection.LEFT:
                        if (pointVel.X > 0) return true;
                        break;
                    case OneWayPlatform.PlatformDirection.RIGHT:
                        if (pointVel.X < 0) return true;
                        break;
                    case OneWayPlatform.PlatformDirection.DOWN:
                        if (pointVel.Y < 0) return true;
                        break;
                    case OneWayPlatform.PlatformDirection.UP:
                        if (pointVel.Y > 0) return true;
                        break;
                }
            }
            return false;
        }
    }
}
