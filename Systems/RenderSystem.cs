using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;
using Platformer.Components;
using System.Collections.Generic;

namespace Platformer.Systems
{
    internal class RenderSystem : EntityDrawSystem
    {
        public SpriteFont DebugFont { get; set; }

        private readonly float DebugThickness = 0.1f;
        private readonly Color DebugColor = Color.Black;
        private readonly OrthographicCamera _camera;
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<Sprite> _spriteMapper;
        private ComponentMapper<Body> _bodyMapper;
        private ComponentMapper<CameraTarget> _cameraTargetMapper;
        private ComponentMapper<GroundedComponent> _groundedMapper;
        public List<(string, Color)> Messages = new();

        public RenderSystem(GraphicsDevice graphicsDevice) : base(Aspect.One(typeof(Sprite), typeof(Body)))
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            _camera = new OrthographicCamera(_graphicsDevice);
            _camera.LookAt(Vector2.Zero);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
            _spriteMapper = mapperService.GetMapper<Sprite>();
            _bodyMapper = mapperService.GetMapper<Body>();
            _cameraTargetMapper = mapperService.GetMapper<CameraTarget>();
            _groundedMapper = mapperService.GetMapper<GroundedComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
#if DEBUG
            DrawGridLines();
#endif
            foreach (var entity in ActiveEntities)
            {
                Vector2 drawPosition = new();
                float drawRotation = 0.0f;
                Vector2 drawScale = new(1, 1);

                if (_transformMapper.Has(entity))
                {
                    var transform = _transformMapper.Get(entity);

                    drawPosition += transform.Position;
                    drawRotation += transform.Rotation;
                    drawScale *= transform.Scale;
                }

                if (_bodyMapper.Has(entity))
                {
                    var body = _bodyMapper.Get(entity);

                    drawPosition += body.GetPosition();
                    drawRotation += body.GetAngle();

                    if (_spriteMapper.Has(entity))
                    {
                        var sprite = _spriteMapper.Get(entity);

                        sprite.Draw(_spriteBatch, drawPosition, drawRotation, drawScale);
                    }
#if DEBUG
                    DrawFixtures(body);
#endif
                }

                if (_cameraTargetMapper.Has(entity))
                {
                    //follow the target horizontally always
                    //lerp to y position when grounded
                    CameraTarget target = _cameraTargetMapper.Get(entity);
                    _camera.Zoom = target.zoom / drawScale.Y;
                    Vector2 delta = (target.offset + drawPosition) - _camera.Center;
                    _camera.Move(Vector2.UnitX * delta.X);
                    if (_groundedMapper.Has(entity) || delta.Y > 0)
                    {
                        _camera.Move(Vector2.UnitY * delta.Y);
                    }
                }
            }
#if DEBUG
            DrawDebugMessages();
#endif
            _spriteBatch.End();
        }
        /// <summary>
        /// Draws 1m x 1m grid
        /// </summary>
        private void DrawGridLines()
        {
            for (float x = -32; x <= 32; x++)
            {
                _spriteBatch.DrawLine(x, -32, x, 32, DebugColor, thickness: DebugThickness);
            }
            for (float y = -32; y <= 32; y++)
            {
                _spriteBatch.DrawLine(-32, y, 32, y, DebugColor, thickness: DebugThickness);
            }
        }

        private void DrawFixtures(Body body)
        {
            foreach (var fixture in body.FixtureList)
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

        private void DrawDebugMessages()
        {
            Vector2 position = _camera.BoundingRectangle.TopLeft;
            position = _camera.ScreenToWorld(position);
            foreach ((string message, Color color) in Messages)
            {
                position.Y += DebugFont.MeasureString(message).Y / _camera.Zoom;
                _spriteBatch.DrawString(DebugFont, message, position, color, -_camera.Rotation, position, 1f / _camera.Zoom, SpriteEffects.None, 0);
            }
            Messages.Clear();
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

                _spriteBatch.DrawLine(position + vertexI, position + vertexJ, DebugColor, thickness: DebugThickness);
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
