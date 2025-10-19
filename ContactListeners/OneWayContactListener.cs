using nkast.Aether.Physics2D.Collision;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using Platformer.Component;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Common;

namespace Platformer.ContactListeners
{
    internal class OneWayContactListener(MonoGame.Extended.ECS.World world) : IContactListener
    {
        //private readonly List<Contact> DisabledContacts = [];
        //private FixedArray2<Vector2> points;

        public bool BeginContact(Contact contact)
        {
            return true;
        }

        public void EndContact(Contact contact)
        {
        }

        public void PreSolve(Contact contact, ref Manifold oldManifold)
        {
            var entityA = world.GetEntity((int)contact.FixtureA.Body.Tag);
            var entityB = world.GetEntity((int)contact.FixtureB.Body.Tag);
            if ((entityA.Has<OneWayPlatform>() && IsContactSolid(oldManifold, entityA.Get<OneWayPlatform>(), contact.FixtureB))
                || (entityB.Has<OneWayPlatform>() && IsContactSolid(oldManifold, entityB.Get<OneWayPlatform>(), contact.FixtureA)))
                return;

            //if (DisabledContacts.Contains(contact))
            //contact.Enabled = false;
        }

        private static bool IsContactSolid(Manifold manifold, OneWayPlatform oneWay, Fixture otherFixture)
        {
            //no points are moving into the one-way's blocking direction, then contact should not be solid
            for (int i = 0; i < manifold.PointCount; i++)
            {
                var pointVel = otherFixture.Body.GetLinearVelocityFromLocalPoint(manifold.Points[i].LocalPoint);
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
