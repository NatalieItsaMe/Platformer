using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tiled;
using Platformer.Models;

namespace Platformer.Factories
{
    class SpriteFactory(ContentManager contentManager)
    {
        private readonly ContentManager _contentManager = contentManager;

        public Sprite BuildSprite(TiledMapTileObject tileObject)
        {
            var id = tileObject.Tile is TiledMapTilesetAnimatedTile animated
                    ? animated.CurrentAnimationFrame.LocalTileIdentifier
                    : tileObject.Tile.LocalTileIdentifier;
            var tileRegion = tileObject.Tileset.GetTileRegion(id);
            var texture = tileObject.Tileset.Texture;
            var textureRegion = new Texture2DRegion(texture, tileRegion.X, tileRegion.Y, tileRegion.Width, tileRegion.Height,
                false, tileRegion.Size, Vector2.Zero, Vector2.One * 0.5f, tileObject.Name);
            return new Sprite(textureRegion);
        }

        public AnimatedSprite BuildAnimatedSprite(string value)
        {
            var model = _contentManager.Load<AnimatedSpriteModel>(value);
            var atlas = BuildTexture2DAtlas(model.TextureAtlas);

            var spriteSheet = new SpriteSheet(model.Name, atlas);

            foreach (var animation in model.Animations)
            {
                spriteSheet.DefineAnimation(animation.Name, animation.BuilderAction);
            }

            if (string.IsNullOrEmpty(model.InitialAnimation))
                return new AnimatedSprite(spriteSheet);

            return new AnimatedSprite(spriteSheet, model.InitialAnimation);
        }


        public Texture2DAtlas BuildTexture2DAtlas(TextureAtlasModel model)
        {
            var texture = _contentManager.Load<Texture2D>(model.Texture);
            var atlas = new Texture2DAtlas(model.Name, texture);

            foreach (var region in model.TextureRegions)
            {
                var bounds = new Rectangle(region.X, region.Y, region.Width, region.Height);
                var size = new Size(region.Width, region.Height);
                Vector2? originNormalized = (region.Origin == null || region.Origin.Length != 2) ? null : new Vector2(region.Origin[0] / region.Width, region.Origin[1] / region.Height);
            
                atlas.CreateRegion(bounds, region.IsRotated, size, Vector2.Zero, originNormalized, region.Name);
            }

            return atlas;
        }
    }
}
