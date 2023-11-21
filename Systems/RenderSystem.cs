using Assimp;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using Platformer.Component;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Platformer.Systems
{
    internal class RenderSystem : EntityDrawSystem, IUpdateSystem
    {
        public SpriteFont DebugFont { get; set; }
        public List<(string, Color)> Messages { get; set; } = new();

        private readonly float DebugThickness = 0.1f;
        private readonly Color DebugColor = Color.Black;
        private readonly OrthographicCamera _camera;
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private TiledMap _tiledMap;
        private TiledMapRenderer _tiledRenderer;

        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<Sprite> _spriteMapper;
        private ComponentMapper<Body> _bodyMapper;
        private ComponentMapper<CameraTarget> _cameraTargetMapper;
        private ComponentMapper<GroundedComponent> _grounded;
        private ComponentMapper<RotationLock> _rotationLock;

        public RenderSystem(GraphicsDevice graphicsDevice) : base(Aspect.One(typeof(Sprite), typeof(Body), typeof(CameraTarget)))
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
            _grounded = mapperService.GetMapper<GroundedComponent>();
            _rotationLock = mapperService.GetMapper<RotationLock>();
        }

        internal void SetTiledMap(TiledMap tiledMap)
        {
            _tiledMap = tiledMap;
            _tiledRenderer = new TiledMapRenderer(_graphicsDevice, tiledMap);
        }

        public void Update(GameTime gameTime)
        {
            _tiledRenderer.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _graphicsDevice.Clear(Color.CornflowerBlue);

            var scale = _tiledMap.GetScale();
            Matrix scaleMatrix = Matrix.CreateScale(scale.X, scale.Y, 1f);
            _tiledRenderer.Draw(scaleMatrix * _camera.GetViewMatrix());

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
#if DEBUG
            //DrawGridLines();
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

                        sprite.Draw(_spriteBatch, drawPosition, _rotationLock.Has(entity) ? 0 : drawRotation, scale);
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
                    _camera.Zoom = target.Zoom / drawScale.Y;
                    Vector2 delta = (target.Offset + drawPosition) - _camera.Center;
                    delta *= 0.1f;

                    _camera.Move(Vector2.UnitX * delta.X);
                    if (_grounded.Has(entity) || delta.Y > 0)
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

        internal OrthographicCamera GetCamera() =>
            _camera;

        /// <summary>
        /// Draws 1m x 1m grid
        /// </summary>
        private void DrawGridLines()
        {
            Color color = Color.White;
            float thickness = 0.05f;
            for (float x = 0; x <= 32; x++)
            {
                _spriteBatch.DrawLine(x, 0, x, 32, color, thickness: thickness);
            }
            for (float y = 0; y <= 32; y++)
            {
                _spriteBatch.DrawLine(0, y, 32, y, color, thickness: thickness);
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
                        DrawPolygon(fixture.Shape as PolygonShape, body.GetPosition(), fixture.Body.GetAngle());
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

        private void DrawPolygon(PolygonShape polygon, Vector2 position, float rotation)
        {
            var points = polygon.Vertices.Select(v => new Vector2(v.X * MathF.Cos(rotation) - v.Y * MathF.Sin(rotation), v.X * MathF.Sin(rotation) + v.Y * MathF.Cos(rotation)) + position).ToArray();
            for (int i = 0; i < polygon.Count; i++)
            {
                int j = (i + 1) % polygon.Count;

                var vertexI = points[i];
                var vertexJ = points[j];

                _spriteBatch.DrawLine(vertexI, vertexJ, DebugColor, thickness: DebugThickness);
            }
        }

        private void DrawEdge(EdgeShape edge, Vector2 position)
        {
            _spriteBatch.DrawLine(position + edge.Vertex1, position + edge.Vertex2, DebugColor, thickness: DebugThickness);
            _spriteBatch.DrawLine(position + edge.Vertex0, position + edge.Vertex3, DebugColor, thickness: DebugThickness);
        }
    }
}
