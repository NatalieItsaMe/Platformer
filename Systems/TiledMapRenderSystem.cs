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
using System.Linq;
using Color = Microsoft.Xna.Framework.Color;

namespace Platformer.Systems
{
    internal class TiledMapRenderSystem : EntityDrawSystem, IUpdateSystem
    {
        public SpriteFont DebugFont { get; set; }

        private TiledMap _tiledMap;
        private TiledMapRenderer _tiledRenderer;
        private Box2dDebugDrawer _box2dDrawer;
        private OrthographicCamera _camera;
        private SpriteBatch _spriteBatch;

        private ComponentMapper<Body> _bodies;
        private ComponentMapper<CameraTarget> _cameraTargets;
        private ComponentMapper<TiledMapTileObject> _tileObjects;
        private ComponentMapper<AnimatedSprite> _animatedSprites;

        public TiledMapRenderSystem() : base(Aspect.All(typeof(Body), typeof(TiledMapTileObject)))
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
            _bodies = mapperService.GetMapper<Body>();
            _cameraTargets = mapperService.GetMapper<CameraTarget>();
            _tileObjects = mapperService.GetMapper<TiledMapTileObject>();
            _animatedSprites = mapperService.GetMapper<AnimatedSprite>();
        }

        public void Update(GameTime gameTime)
        {
            _tiledRenderer.Update(gameTime);
            _animatedSprites.Components.ToList().ForEach(s => s.Update(gameTime));

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
                var body = _bodies.Get(entity);
                var rotation = body.GetAngle();
                var position = body.GetWorldCenter();

                var obj = _tileObjects.Get(entity);
                var id = obj.Tile is TiledMapTilesetAnimatedTile animated
                    ? animated.CurrentAnimationFrame.LocalTileIdentifier
                    : obj.Tile.LocalTileIdentifier;
                var region = obj.Tileset.GetTileRegion(id);
                var texture = obj.Tileset.Texture;

                Vector2 origin = region.Size.ToVector2() / 2;
                _spriteBatch.Draw(texture, position, region, Color.White, rotation, origin, scale.Y, SpriteEffects.None, 1f);

                if (_cameraTargets.Has(entity))
                {
                    CameraTarget target = _cameraTargets.Get(entity);
                    _camera.Zoom = target.Zoom;
                    _camera.LerpToPosition(position + target.Offset);
                }
            }
#if DEBUG
            foreach(var body in _bodies.Components.Where(b => b != null))
                _box2dDrawer.Draw(body);
#endif
            _spriteBatch.End();
        }

        internal Vector2 GetWorldCoordinates(float x, float y) => _camera.ScreenToWorld(x, y);
    }
}
