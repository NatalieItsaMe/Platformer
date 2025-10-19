using nkast.Aether.Physics2D.Collision;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using Platformer.Component;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Graphics;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Common;

namespace Platformer.ContactListeners
{
    internal class SpringContactListener(MonoGame.Extended.ECS.World world) : IContactListener
    {
        private readonly Dictionary<Contact, float> SpringContacts = [];
        private FixedArray2<Vector2> points;

        public bool BeginContact(Contact contact)
        {
            contact.GetWorldManifold(out _, out points);
            DerestituteSpringContact(contact);
            return true;
        }

        public void EndContact(Contact contact)
        {
            //reset the default state of the contact in case it comes back for more
            SpringContacts.Remove(contact);
            contact.Enabled = true;
        }

        public void PreSolve(Contact contact, ref Manifold oldManifold) 
        {
            if (SpringContacts.TryGetValue(contact, out float value))
                contact.Restitution = value;
        }

        private void AnimateSpring(Fixture springFixture)
        {
            var entity = world.GetEntity((int)springFixture.Body.Tag);
            if (!entity.Has<AnimatedSprite>())
                return;
            
            var sprite = entity.Get<AnimatedSprite>();

            sprite.SetAnimation("sproing");
        }

        private void DerestituteSpringContact(Contact contact)
        {
            //Restitution of the spring should only apply to
            //collisions with the face of the spring
            if (world.GetEntity((int)contact.FixtureA.Body.Tag).Has<SpringComponent>())
            {
                UpdateSpringyContact(contact, contact.FixtureA, contact.FixtureB);
            }
            if (world.GetEntity((int)contact.FixtureB.Body.Tag).Has<SpringComponent>())
            {
                UpdateSpringyContact(contact, contact.FixtureB, contact.FixtureA);
            }
        }

        private void UpdateSpringyContact(Contact contact, Fixture springFixture, Fixture otherFixture)
        {
            float minY = ((PolygonShape)springFixture.Shape).Vertices.Min(v => v.Y);
            for (int i = 0; i < contact.Manifold.PointCount; i++)
            {
                //if the other fixture is moving into the springy top
                //return without changing the restitution
                var springPoint = springFixture.Body.GetLocalPoint(points[i]);

                if (springPoint.Y < minY)
                {
                    AnimateSpring(springFixture);
                    return;
                }
            }

            //cancel out the spring's restitution
            SpringContacts.Add(contact, otherFixture.Restitution);
            contact.Restitution = otherFixture.Restitution;
        }
    }
}
