using nkast.Aether.Physics2D.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using MonoGame.Extended;

namespace Platformer.Systems
{
    internal class SpriteDrawSystem(SpriteBatch spriteBatch) : EntityDrawSystem(Aspect.All(typeof(Body)).One(typeof(AnimatedSprite), typeof(Sprite)))
    {
        private readonly Matrix3x2 scale = Matrix3x2.CreateFrom(Vector2.Zero, 0f, new Vector2(16f, 16f), Vector2.Zero);
        private ComponentMapper<Body> _bodies;
        private ComponentMapper<Sprite> _sprites;
        private ComponentMapper<AnimatedSprite> _animatedSprites;

        public override void Initialize(IComponentMapperService mapperService)
        {
            _bodies = mapperService.GetMapper<Body>();
            _sprites = mapperService.GetMapper<Sprite>();
            _animatedSprites = mapperService.GetMapper<AnimatedSprite>();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                var body = _bodies.Get(entity);
                var rotation = body.Rotation;
                var position = scale.Transform(body.GetTransform().p);

                if (_sprites.Has(entity))
                {
                    var sprite = _sprites.Get(entity);
                    spriteBatch.Draw(sprite, position, rotation);
                }

                if (_animatedSprites.Has(entity))
                {
                    var sprite = _animatedSprites.Get(entity);
                    sprite.Update(gameTime);
                    spriteBatch.Draw(sprite, position, rotation);
                }
            }
        }
    }
}
