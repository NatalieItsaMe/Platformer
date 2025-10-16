using nkast.Aether.Physics2D.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using Platformer.Component;
using System.Linq;

namespace Platformer.Systems
{
    internal class TiledMapRenderSystem(SpriteBatch spriteBatch) : EntityDrawSystem(Aspect.All(typeof(Body)).One(typeof(AnimatedSprite), typeof(Sprite), typeof(TiledMapTileObject), typeof(CameraTarget))), IUpdateSystem
    {
        public SpriteFont DebugFont { get; set; }
        public Box2dDebugDrawer PhysicsDebugDrawer { get; set; }

        private TiledMapRenderer _tiledRenderer;
        private readonly OrthographicCamera _camera = new OrthographicCamera(spriteBatch.GraphicsDevice);
        private RectangleF CameraBounds = new();
        private Vector2 Scale = new();

        private ComponentMapper<Body> _bodies;
        private ComponentMapper<CameraTarget> _cameraTargets;
        private ComponentMapper<TiledMapTileObject> _tileObjects;
        private ComponentMapper<Sprite> _sprites;
        private ComponentMapper<AnimatedSprite> _animatedSprites;

        public void SetTiledMap(TiledMap tiledMap)
        {
            _tiledRenderer = new TiledMapRenderer(spriteBatch.GraphicsDevice, tiledMap);
            CameraBounds.Width = tiledMap.Width;
            CameraBounds.Height = tiledMap.Height;
            Scale = tiledMap.GetScale();
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _bodies = mapperService.GetMapper<Body>();
            _cameraTargets = mapperService.GetMapper<CameraTarget>();
            _tileObjects = mapperService.GetMapper<TiledMapTileObject>();
            _sprites = mapperService.GetMapper<Sprite>();
            _animatedSprites = mapperService.GetMapper<AnimatedSprite>();
        }

        public void Update(GameTime gameTime)
        {
            _tiledRenderer.Update(gameTime);
            _animatedSprites.Components.Where(s => s != null)
                .ToList()
                .ForEach(s => s.Update(gameTime));

            ClampCameraWithinBounds();
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix scaleMatrix = Matrix.CreateScale(Scale.X, Scale.Y, 1f);

            _tiledRenderer.Draw(scaleMatrix * _camera.GetViewMatrix());

            spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
            foreach (var entity in ActiveEntities)
            {
                var body = _bodies.Get(entity);
                var rotation = body.Rotation;
                var position = body.Position;

                if (_tileObjects.Has(entity))
                {
                    var obj = _tileObjects.Get(entity);
                    var id = obj.Tile is TiledMapTilesetAnimatedTile animated
                        ? animated.CurrentAnimationFrame.LocalTileIdentifier
                        : obj.Tile.LocalTileIdentifier;
                    var region = obj.Tileset.GetTileRegion(id);
                    var texture = obj.Tileset.Texture;

                    Vector2 origin = region.Size.ToVector2() / 2;
                    spriteBatch.Draw(texture, position, region, Color.White, rotation, origin, Scale.Y, SpriteEffects.None, 1f);
                }

                if (_sprites.Has(entity))
                {
                    var sprite = _sprites.Get(entity);
                    spriteBatch.Draw(sprite, position, rotation, Scale);
                }

                if (_animatedSprites.Has(entity))
                {
                    var sprite = _animatedSprites.Get(entity);
                    spriteBatch.Draw(sprite, position, rotation, Scale);
                }

                if (_cameraTargets.Has(entity))
                {
                    CameraTarget target = _cameraTargets.Get(entity);
                    _camera.Zoom = target.Zoom;
                    _camera.LerpToPosition(position + target.Offset);
                }
            }

            PhysicsDebugDrawer.Draw();
            spriteBatch.End();
        }

        internal Vector2 GetWorldCoordinates(float x, float y) => _camera.ScreenToWorld(x, y);

        public void ClampCameraWithinBounds()
        {
            Vector2 d = new();
            if (_camera.BoundingRectangle.Width > CameraBounds.Width)
            {
                d.X = (CameraBounds.Center.X - _camera.BoundingRectangle.Center.X);
            }
            else
            {
                if (_camera.BoundingRectangle.Left < CameraBounds.Left)
                {
                    d.X = (CameraBounds.Left - _camera.BoundingRectangle.Left);
                }
                if (_camera.BoundingRectangle.Right > CameraBounds.Right)
                {
                    d.X = (CameraBounds.Right - _camera.BoundingRectangle.Right);
                }
            }
            if (_camera.BoundingRectangle.Height > CameraBounds.Height)
            {
                d.Y = (CameraBounds.Center.Y - _camera.BoundingRectangle.Center.Y);
            }
            else
            {
                if (_camera.BoundingRectangle.Top < CameraBounds.Top)
                {
                    d.Y = (CameraBounds.Top - _camera.BoundingRectangle.Top);
                }
                if (_camera.BoundingRectangle.Bottom > CameraBounds.Bottom)
                {
                    d.Y = (CameraBounds.Bottom - _camera.BoundingRectangle.Bottom);
                }
            }
            _camera.Move(d);
        }
    }
}
