using Box2DSharp.Collision.Collider;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Platformer.Component;
using System.Collections.Generic;
using System.Numerics;

namespace Platformer
{
    internal class Box2dContactListener : EntitySystem, IContactListener
    {
        private const float GroundNormal = 0.5f;
        private readonly List<Contact> DisabledContacts = new();
        private WorldManifold worldManifold;

        private ComponentMapper<OneWayPlatform> oneWays;
        private ComponentMapper<GroundedComponent> grounded;
        private ComponentMapper<SpringComponent> springs;

        public Box2dContactListener() : base(Aspect.All())
        { }

        public override void Initialize(IComponentMapperService mapperService)
        {
            oneWays = mapperService.GetMapper<OneWayPlatform>();
            grounded = mapperService.GetMapper<GroundedComponent>();
            springs = mapperService.GetMapper<SpringComponent>();
        }

        public void BeginContact(Contact contact)
        {
            contact.GetWorldManifold(out worldManifold);

            DisableOneWayContact(contact);

            if (contact.IsEnabled)
            {
                AttachGroundedComponents(contact);
            }
        }

        private void AttachGroundedComponents(Contact contact)
        {
            //fixure A is the start, -normal points towards A
            if (worldManifold.Normal.Y > GroundNormal)
            {
                grounded.Put((int)contact.FixtureA.Body.UserData, new GroundedComponent(contact));
            }
            //fixure B is the end, normal points towards B
            if (-worldManifold.Normal.Y > GroundNormal)
            {
                grounded.Put((int)contact.FixtureB.Body.UserData, new GroundedComponent(contact));
            }
        }

        public void EndContact(Contact contact)
        {
            //reset the default state of the contact in case it comes back for more
            DisabledContacts.Remove(contact);
            contact.SetEnabled(true);

            DetachGroundedComponents(contact, contact.FixtureA.Body);
            DetachGroundedComponents(contact, contact.FixtureB.Body);
        }

        public void PreSolve(Contact contact, in Manifold oldManifold) 
        {
            DisabledContacts.ForEach(c => c.SetEnabled(false)); 
        }

        public void PostSolve(Contact contact, in ContactImpulse impulse) 
        { 

        }

        private void DetachGroundedComponents(Contact contact, Body body)
        {
            int e = (int)body.UserData;
            //fixure A is the start, -normal points towards A
            if (grounded.Has(e) && grounded.Get(e).Contact == contact)
            {
                grounded.Delete(e);
            }
        }

        private void DisableOneWayContact(Contact contact)
        {
            //check if one of the fixtures is the platform
            Fixture platformFixture = null;
            Fixture otherFixture = null;
            if (oneWays.Has((int)contact.FixtureA.Body.UserData))
            {
                platformFixture = contact.FixtureA;
                otherFixture = contact.FixtureB;
            }
            else if (oneWays.Has((int)contact.FixtureB.Body.UserData))
            {
                platformFixture = contact.FixtureB;
                otherFixture = contact.FixtureA;
            }

            if (platformFixture == null)
                return;

            OneWayPlatform oneWay = oneWays.Get((int)platformFixture.Body.UserData);
            for (int i = 0; i < contact.Manifold.PointCount; i++)
            {
                Vector2 pointVel = otherFixture.Body.GetLinearVelocityFromWorldPoint(worldManifold.Points[i]);
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
            contact.SetEnabled(false);
            DisabledContacts.Add(contact);
        }
    }
}
