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
using System.Collections.Generic;

namespace Platformer.Systems
{
    internal class TiledMapRenderSystem : EntityDrawSystem, IUpdateSystem
    {
        public SpriteFont DebugFont { get; set; }
        public List<(string, Color)> Messages { get; set; } = new();

        private TiledMap _tiledMap;
        private TiledMapRenderer _tiledRenderer;
        private Box2dDebugDrawer _box2dDrawer;
        private OrthographicCamera _camera;
        private SpriteBatch _spriteBatch;

        private ComponentMapper<Sprite> _sprites;
        private ComponentMapper<Body> _bodies;
        private ComponentMapper<CameraTarget> _cameraTargets;

        public TiledMapRenderSystem() : base(Aspect.One(typeof(Sprite), typeof(Body)))
        { }

        public void SetTiledMap(GraphicsDevice graphicsDevice, TiledMap tiledMap)
        {
            _tiledMap = tiledMap;
            _tiledRenderer = new TiledMapRenderer(graphicsDevice, tiledMap);
            _camera = new OrthographicCamera(graphicsDevice);
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _box2dDrawer = new Box2dDebugDrawer(_spriteBatch);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _sprites = mapperService.GetMapper<Sprite>();
            _bodies = mapperService.GetMapper<Body>();
            _cameraTargets = mapperService.GetMapper<CameraTarget>();
        }

        public void Update(GameTime gameTime)
        {
            _tiledRenderer.Update(gameTime);

            _camera.ClampWithinBounds(new(0, 0, _tiledMap.Width, _tiledMap.Height));
        }

        public override void Draw(GameTime gameTime)
        {
            var scale = _tiledMap.GetScale();
            Matrix scaleMatrix = Matrix.CreateScale(scale.X, scale.Y, 1f);

            _tiledRenderer.Draw(scaleMatrix * _camera.GetViewMatrix());

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
            foreach (var entity in ActiveEntities)
            {
                Vector2 drawPosition = new();
                float drawRotation = 0.0f;

                if (_bodies.Has(entity))
                {
                    var body = _bodies.Get(entity);

                    drawPosition += body.GetWorldCenter();
                    drawRotation += body.GetAngle();

                    _box2dDrawer.Draw(body);
                }

                if (_sprites.Has(entity))
                {
                    var sprite = _sprites.Get(entity);

                    sprite.Draw(_spriteBatch, drawPosition, drawRotation, scale);
                }

                if (_cameraTargets.Has(entity))
                {
                    CameraTarget target = _cameraTargets.Get(entity);
                    _camera.Zoom = target.Zoom;
                    _camera.LerpToPosition(drawPosition + target.Offset);
                }
            }
            _spriteBatch.End();
        }

        internal Vector2 GetWorldCoordinates(float x, float y) => _camera.ScreenToWorld(x, y);
    }
}
