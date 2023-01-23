using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;

namespace Platformer.Systems
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
            foreach (var entity in ActiveEntities)
            {
                var transform = _transformMapper.Get(entity);

                if(_spriteMapper.Has(entity))
                {
                    var sprite = _spriteMapper.Get(entity);

                    sprite.Draw(_spriteBatch, transform.Position, transform.Rotation, transform.Scale);
                }
                if(_bodyMapper.Has(entity))
                {
                    var body = _bodyMapper.Get(entity);

                    foreach(var fixture in body.FixtureList)
                    {
                        if(fixture.ShapeType == ShapeType.Circle)
                        {
                            var circle = fixture.Shape as CircleShape;

                            _spriteBatch.DrawCircle(circle.Position.X, circle.Position.Y, circle.Radius, 1, Color.Cyan);
                        }

                        if(fixture.ShapeType == ShapeType.Polygon)
                        {
                            var polygon = fixture.Shape as PolygonShape;

                            for(int i=0; i < polygon.Count; i++)
                            {
                                int j = (i + 1) % polygon.Count;

                                var vertexI = polygon.Vertices[i];
                                var vertexJ = polygon.Vertices[j];

                                _spriteBatch.DrawLine(vertexI, vertexJ, Color.Cyan);
                            }

                        }

                        if(fixture.ShapeType == ShapeType.Edge)
                        {
                            var edge = fixture.Shape as EdgeShape;

                            _spriteBatch.DrawLine(edge.Vertex0, edge.Vertex1, Color.Cyan);
                            _spriteBatch.DrawLine(edge.Vertex1, edge.Vertex2, Color.Cyan);
                            _spriteBatch.DrawLine(edge.Vertex2, edge.Vertex3, Color.Cyan);
                            _spriteBatch.DrawLine(edge.Vertex3, edge.Vertex0, Color.Cyan);
                        }

                    }

                }
            }
            _spriteBatch.End();
        }
    }
}
