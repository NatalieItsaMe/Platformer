using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;

namespace Platformer
{
    internal class RenderSystem : EntityDrawSystem
    {
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<Sprite> _spriteMapper;
        private ComponentMapper<Body> _bodyMapper;

        public RenderSystem(GraphicsDevice graphicsDevice) : base(Aspect.All(typeof(Transform2)).One(typeof(Sprite), typeof(Body)))
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);

        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
            _spriteMapper = mapperService.GetMapper<Sprite>();
            _bodyMapper = mapperService.GetMapper<Body>();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            foreach(var entity in ActiveEntities)
            {
                var transform = _transformMapper.Get(entity);

                if(_spriteMapper.Has(entity))
                {
                    var sprite = _spriteMapper.Get(entity);

                    sprite.Draw(_spriteBatch, transform.Position, transform.Rotation, transform.Scale);
                }
                if(_bodyMapper.Has(entity))
                {
                    var rectangle = _bodyMapper.Get(entity).Rectangle;
                    rectangle.Offset(transform.Position.X, transform.Position.Y);
                    rectangle.Inflate(transform.Scale.X, transform.Scale.Y);

                    _spriteBatch.DrawRectangle(rectangle, Color.Black);
                }
                
            }
            _spriteBatch.End();
        }
    }
}
