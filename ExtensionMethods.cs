namespace Platformer
{
    public static class ExtensionMethods
    {
        public static Microsoft.Xna.Framework.Vector2 ToVector2(this MonoGame.Extended.SizeF s) =>
            new Microsoft.Xna.Framework.Vector2(s.Width, s.Height);

        public static Microsoft.Xna.Framework.Vector2 ToPhysics2D(this Microsoft.Xna.Framework.Vector2 v) =>
            new Microsoft.Xna.Framework.Vector2(v.X, v.Y);

        public static Microsoft.Xna.Framework.Vector2 GetScale(this MonoGame.Extended.Tiled.TiledMap t) =>
             new Microsoft.Xna.Framework.Vector2(1f / t.TileWidth, 1f / t.TileHeight);

        public static void LerpToPosition(this MonoGame.Extended.OrthographicCamera camera, Microsoft.Xna.Framework.Vector2 position)
        {
            Microsoft.Xna.Framework.Vector2 delta = position - camera.BoundingRectangle.Center.ToNumerics();

            if (camera.BoundingRectangle.Contains(position))
            {
                delta *= 0.1f;
            }
            camera.Move(delta);
        }
    }
}
