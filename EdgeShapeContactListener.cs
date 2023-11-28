using Box2DSharp.Collision.Collider;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using MonoGame.Extended.Entities;
using System;

namespace Platformer
{
    internal class EdgeShapeContactListener 
    {
        private MonoGame.Extended.Entities.World _world;

        public EdgeShapeContactListener(MonoGame.Extended.Entities.World world)
        {
            _world = world;
        }

        public void BeginContact(Contact contact)
        {
        }

        public void EndContact(Contact contact)
        {
        }


        public void PreSolve(Contact contact, in Manifold oldManifold)
        {
        }
    }
}
