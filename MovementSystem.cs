using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;

namespace Platformer
{
    internal class MovementSystem : EntityUpdateSystem
    {
        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<Body> _bodyMapper;

        public MovementSystem() : base(Aspect.All(typeof(Transform2), typeof(Body)))
        {

        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
            _bodyMapper = mapperService.GetMapper<Body>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach(var entity in ActiveEntities)
            {
                Body body = _bodyMapper.Get(entity);

                if(MathF.Abs(body.Velocity.X) < body.MaxSpeed.X)
                    body.Velocity += Vector2.UnitX * body.Acceleration * gameTime.GetElapsedSeconds();
                if(MathF.Abs(body.Velocity.Y) < body.MaxSpeed.Y)
                    body.Velocity += Vector2.UnitY * body.Acceleration * gameTime.GetElapsedSeconds();

                _transformMapper.Get(entity).Position += body.Velocity * gameTime.GetElapsedSeconds();
            }
        }
    }
}
