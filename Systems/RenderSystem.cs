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
        private readonly float DebugThickness = 0.1f;
        private readonly Color DebugColor = Color.Black;
        private readonly OrthographicCamera _camera;
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<Sprite> _spriteMapper;
        private ComponentMapper<Body> _bodyMapper;

        public RenderSystem(GraphicsDevice graphicsDevice) : base(Aspect.All(typeof(Transform2)).One(typeof(Sprite), typeof(Body)))
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            _camera = new OrthographicCamera(_graphicsDevice);
            _camera.LookAt(new(0, 0));
            _camera.ZoomIn(8);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
            _spriteMapper = mapperService.GetMapper<Sprite>();
            _bodyMapper = mapperService.GetMapper<Body>();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
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

                    if(_spriteMapper.Has(entity))
                    {
                        var sprite = _spriteMapper.Get(entity);

                        sprite.Draw(_spriteBatch, drawPosition, drawRotation, drawScale);
                    }

                    foreach(var fixture in body.FixtureList)
                    {
                        switch (fixture.ShapeType)
                        {
                            case ShapeType.Circle:
                                DrawCircle(fixture.Shape as CircleShape, body.GetPosition());
                                break;
                            case ShapeType.Polygon:
                                DrawPolygon(fixture.Shape as PolygonShape, body.GetPosition());
                                break;
                            case ShapeType.Edge:
                                DrawEdge(fixture.Shape as EdgeShape, body.GetPosition());
                                break;
                        }
                    }
                }
            }
            _spriteBatch.End();
        }

        private void DrawCircle(CircleShape circle, Vector2 position)
        {
            _spriteBatch.DrawCircle(position + circle.Position, circle.Radius, 24, DebugColor, thickness: DebugThickness);
        }

        private void DrawPolygon(PolygonShape polygon, Vector2 position)
        {
            for (int i = 0; i < polygon.Count; i++)
            {
                int j = (i + 1) % polygon.Count;

                var vertexI = polygon.Vertices[i];
                var vertexJ = polygon.Vertices[j];

                _spriteBatch.DrawLine(position + vertexI, position + vertexJ, DebugColor, thickness:DebugThickness);
            }
        }

        private void DrawEdge(EdgeShape edge, Vector2 position)
        {
            _spriteBatch.DrawLine(position + edge.Vertex0, position + edge.Vertex1, DebugColor, thickness: DebugThickness);
            _spriteBatch.DrawLine(position + edge.Vertex1, position + edge.Vertex2, DebugColor, thickness: DebugThickness);
            _spriteBatch.DrawLine(position + edge.Vertex2, position + edge.Vertex3, DebugColor, thickness: DebugThickness);
            _spriteBatch.DrawLine(position + edge.Vertex3, position + edge.Vertex0, DebugColor, thickness: DebugThickness);
        }
    }
}
