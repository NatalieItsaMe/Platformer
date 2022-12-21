using System;
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
                var bodyA = _bodyMapper.Get(entityA);
                RectangleF rectangleA = GetTransformedRectangle(entityA);

                for (int b = a + 1; b < ActiveEntities.Count; b++)
                {
                    var entityB = ActiveEntities[b];
                    var bodyB = _bodyMapper.Get(entityB);
                    RectangleF rectangleB = GetTransformedRectangle(entityB);

                    if (rectangleA.Intersects(rectangleB))
                    {
                        var intersection = rectangleA.Intersection(rectangleB);

                    }
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
