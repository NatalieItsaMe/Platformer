using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS.Systems;
using nkast.Aether.Physics2D.Dynamics;
using Platformer.ContactListeners;
using System.Collections.Generic;

namespace Platformer.Systems
{
    internal class PhysicsSystem : World, IUpdateSystem
    {
        private static readonly Vector2 DEFAULT_GRAVITY = new (0, 21f);

        public PhysicsSystem() : base(DEFAULT_GRAVITY)
        {
        }

        public void Update(GameTime gameTime)
        {
            Step(gameTime.ElapsedGameTime);
            ClearForces();
        }

        internal IEnumerable<Body> GetBodiesAt(float x, float y)
        {
            var point = new Vector2(x, y);
            var aabb = new nkast.Aether.Physics2D.Collision.AABB(point, point);

            var hits = new List<Body>();
            QueryAABB(f =>
            {
                hits.Add(f.Body);
                return true;
            }, aabb);
            return hits;
        }

        public void RegisterContactListener(IContactListener contactListener)
        {
            ContactManager.BeginContact += contactListener.BeginContact;
            ContactManager.EndContact += contactListener.EndContact;
            ContactManager.PreSolve += contactListener.PreSolve;
        }

        public void Initialize(MonoGame.Extended.ECS.World world)
        {
            
        }

        public void Dispose()
        {
            base.Clear();
        }
    }
}
