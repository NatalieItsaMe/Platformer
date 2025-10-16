using nkast.Aether.Physics2D.Collision;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using Platformer.Component;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Common;

namespace Platformer
{
    internal class Box2dContactListener : EntitySystem
    {
        private const float GroundNormal = 0.5f;
        private readonly List<Contact> DisabledContacts = new();
        private readonly Dictionary<Contact, float> SpringContacts = new();

        private ComponentMapper<OneWayPlatform> oneWays;
        private ComponentMapper<GroundedComponent> grounded;
        private ComponentMapper<SpringComponent> springs;
        private ComponentMapper<AnimatedSprite> animatedSprites;
        private FixedArray2<Vector2> points;
        private Vector2 normal;

        public Box2dContactListener() : base(Aspect.All())
        { }

        public override void Initialize(IComponentMapperService mapperService)
        {
            oneWays = mapperService.GetMapper<OneWayPlatform>();
            grounded = mapperService.GetMapper<GroundedComponent>();
            springs = mapperService.GetMapper<SpringComponent>();
            animatedSprites = mapperService.GetMapper<AnimatedSprite>();
        }

        public void BeginContact(Contact contact)
        {
            contact.GetWorldManifold(out normal, out points);

            DisableOneWayContact(contact);

            if (contact.Enabled)
            {
                DerestituteSpringContact(contact);
                AttachGroundedComponents(contact);
            }
        }

        public void EndContact(Contact contact)
        {
            //reset the default state of the contact in case it comes back for more
            SpringContacts.Remove(contact);
            DisabledContacts.Remove(contact);
            contact.Enabled = true;
            DetachGroundedComponents(contact, (int)contact.FixtureA.Body.Tag);
            DetachGroundedComponents(contact, (int)contact.FixtureB.Body.Tag);
        }

        public void PreSolve(Contact contact, in Manifold oldManifold) 
        {
            if (DisabledContacts.Contains(contact))
                contact.Enabled = false;
            if (SpringContacts.ContainsKey(contact))
                contact.Restitution = SpringContacts[contact];
        }

        private void AnimateSpring(Fixture springFixture)
        {
            if (!animatedSprites.Has((int)springFixture.Body.Tag))
                return;
            
            var sprite = animatedSprites.Get((int)springFixture.Body.Tag);

            sprite.SetAnimation("sproing");
        }

        private void AttachGroundedComponents(Contact contact)
        {
            //fixure A is the start, -normal points towards A
            if (normal.Y > GroundNormal)
            {
                grounded.Put((int)contact.FixtureA.Body.Tag, new GroundedComponent(contact));
            }
            //fixure B is the end, normal points towards B
            if (-normal.Y > GroundNormal)
            {
                grounded.Put((int)contact.FixtureB.Body.Tag, new GroundedComponent(contact));
            }
        }

        private void DetachGroundedComponents(Contact contact, int entityID)
        {
            if (grounded.Has(entityID) && grounded.Get(entityID).Contact == contact)
            {
                grounded.Delete(entityID);
            }
        }

        private void DisableOneWayContact(Contact contact)
        {
            //check if one of the fixtures is the platform
            Fixture platformFixture = null;
            Fixture otherFixture = null;
            if (oneWays.Has((int)contact.FixtureA.Body.Tag))
            {
                platformFixture = contact.FixtureA;
                otherFixture = contact.FixtureB;
            }
            else if (oneWays.Has((int)contact.FixtureB.Body.Tag))
            {
                platformFixture = contact.FixtureB;
                otherFixture = contact.FixtureA;
            }

            if (platformFixture == null)
                return;

            OneWayPlatform oneWay = oneWays.Get((int)platformFixture.Body.Tag);
            for (int i = 0; i < contact.Manifold.PointCount; i++)
            {
                var pointVel = otherFixture.Body.GetLinearVelocityFromWorldPoint(points[i]);
                switch (oneWay.Direction)
                {
                    case OneWayPlatform.PlatformDirection.LEFT:
                        if (pointVel.X > 0) return;
                        break;
                    case OneWayPlatform.PlatformDirection.RIGHT:
                        if (pointVel.X < 0) return;
                        break;
                    case OneWayPlatform.PlatformDirection.DOWN:
                        if (pointVel.Y < 0)  return;
                        break;
                    case OneWayPlatform.PlatformDirection.UP:
                        if (pointVel.Y > 0) return;
                        break;
                }
            }

            //no points are moving downward, contact should not be solid
            contact.Enabled = false;
            DisabledContacts.Add(contact);
        }

        private void DerestituteSpringContact(Contact contact)
        {
            //Restitution of the spring should only apply to
            //collisions with the face of the spring
            Fixture springFixture = null;
            Fixture otherFixture = null;
            if (springs.Has((int)contact.FixtureA.Body.Tag))
            {
                springFixture = contact.FixtureA;
                otherFixture = contact.FixtureB;
            }
            if (springs.Has((int)contact.FixtureB.Body.Tag))
            {
                springFixture = contact.FixtureB;
                otherFixture = contact.FixtureA;
            }

            if (springFixture == null) return;

            for (int i = 0; i < contact.Manifold.PointCount; i++)
            {
                //if the other fixture is moving into the springy top
                //return without changing the restitution
                var springPoint = springFixture.Body.GetLocalPoint(points[i]);
                float minY = ((PolygonShape)springFixture.Shape).Vertices.Min(v => v.Y);

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
