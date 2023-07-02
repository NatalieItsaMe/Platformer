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
        private readonly Color DebugColor = Color.Black;
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

                Vector2 drawPosition = transform.Position;
                float drawRotation = transform.Rotation;
                Vector2 drawScale = transform.Scale;

                if(_bodyMapper.Has(entity))
                {
                    var body = _bodyMapper.Get(entity);

                    drawPosition += body.GetPosition();
                    drawRotation += body.GetAngle();

                    foreach(var fixture in body.FixtureList)
                    {
                        switch (fixture.ShapeType)
                        {
                            case ShapeType.Circle:
                                DrawCircle(fixture.Shape as CircleShape);
                                break;
                            case ShapeType.Polygon:
                                DrawPolygon(fixture.Shape as PolygonShape);
                                break;
                            case ShapeType.Edge:
                                DrawEdge(fixture.Shape as EdgeShape);
                                break;
                        }
                    }
                }
                if(_spriteMapper.Has(entity))
                {
                    var sprite = _spriteMapper.Get(entity);

                    sprite.Draw(_spriteBatch, drawPosition, drawRotation, drawScale);
                }
            }
            _spriteBatch.End();
        }

        private void DrawCircle(CircleShape circle)
        {
            _spriteBatch.DrawCircle(circle.Position.X, circle.Position.Y, circle.Radius, 1, DebugColor);
        }

        private void DrawPolygon(PolygonShape polygon)
        {
            for (int i = 0; i < polygon.Count; i++)
            {
                int j = (i + 1) % polygon.Count;

                var vertexI = polygon.Vertices[i];
                var vertexJ = polygon.Vertices[j];

                _spriteBatch.DrawLine(vertexI, vertexJ, DebugColor);
            }
        }

        private void DrawEdge(EdgeShape edge)
        {
            _spriteBatch.DrawLine(edge.Vertex0, edge.Vertex1, DebugColor);
            _spriteBatch.DrawLine(edge.Vertex1, edge.Vertex2, DebugColor);
            _spriteBatch.DrawLine(edge.Vertex2, edge.Vertex3, DebugColor);
            _spriteBatch.DrawLine(edge.Vertex3, edge.Vertex0, DebugColor);
        }
    }
}
