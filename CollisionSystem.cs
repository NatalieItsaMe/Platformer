using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;

namespace Platformer
{
    internal class CollisionSystem : EntityUpdateSystem
    {
        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<Body> _bodyMapper;

        public CollisionSystem() : base(Aspect.All(typeof(Transform2), typeof(Body)))
        {

        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
            _bodyMapper = mapperService.GetMapper<Body>();
        }

        public override void Update(GameTime gameTime)
        {
            for(int a = 0; a < ActiveEntities.Count - 1; a ++)
            {
                var entityA = ActiveEntities[a];
                for (int b = a + 1; b < ActiveEntities.Count; b++)
                {
                    var entityB = ActiveEntities[b];
                    CollisionCheck(entityA, entityB);
                    CollisionCheck(entityB, entityA);
                }
            }
        }

        private void CollisionCheck(int a, int b)
        {
            Transform2 transformA = _transformMapper.Get(a);
            Transform2 transformB = _transformMapper.Get(b);
            Body bodyA = _bodyMapper.Get(a);
            Body bodyB = _bodyMapper.Get(b);
            RectangleF rectangleA = new RectangleF(bodyA.Rectangle.Position, bodyA.Rectangle.Size);
            RectangleF rectangleB = new RectangleF(bodyB.Rectangle.Position, bodyB.Rectangle.Size);
            rectangleA.Offset(transformA.Position);
            rectangleB.Offset(transformB.Position);
            rectangleA.Inflate(transformA.Scale.X, transformA.Scale.Y);
            rectangleB.Inflate(transformB.Scale.X, transformB.Scale.Y);

            if (rectangleA.Intersects(rectangleB))
            {
                if (rectangleA.Right > rectangleB.Left && rectangleA.Right < rectangleB.Right && bodyA.Velocity.X > 0)
                {
                    bodyA.Acceleration = bodyA.Acceleration.SetX(0);
                    bodyA.Velocity = bodyA.Velocity.SetX(rectangleB.Left - rectangleA.Right);
                }
                if (rectangleA.Left < rectangleB.Right && rectangleA.Left > rectangleB.Left && bodyA.Velocity.X < 0)
                {
                    bodyA.Acceleration = bodyA.Acceleration.SetX(0);
                    bodyA.Velocity = bodyA.Velocity.SetX(rectangleB.Right - rectangleA.Left);
                }
                if (rectangleA.Bottom > rectangleB.Top && rectangleA.Bottom < rectangleB.Bottom && bodyA.Velocity.Y > 0)
                {
                    bodyA.Acceleration = bodyA.Acceleration.SetY(0);
                    bodyA.Velocity = bodyA.Velocity.SetY(rectangleB.Top - rectangleA.Bottom);
                }
                if (rectangleA.Top < rectangleB.Bottom && rectangleA.Top > rectangleB.Top && bodyA.Velocity.Y < 0)
                {
                    bodyA.Acceleration = bodyA.Acceleration.SetY(0);
                    bodyA.Velocity = bodyA.Velocity.SetY(rectangleB.Bottom - rectangleA.Top);
                }
            }
        }

        private RectangleF GetTransformedRectangle(int entityA)
        {
            var transformA = _transformMapper.Get(entityA);
            var rectangleA = _bodyMapper.Get(entityA).Rectangle;
            rectangleA.Offset(transformA.Position.X, transformA.Position.Y);
            rectangleA.Inflate(transformA.Scale.X, transformA.Scale.Y);
            return rectangleA;
        }
    }
}
