namespace Platformer
{
    public static class ExtensionMethods
    {
        public static System.Numerics.Vector2 ToNumerics(this MonoGame.Extended.SizeF s) =>
            new System.Numerics.Vector2(s.Width, s.Height);

        public static System.Numerics.Vector2 GetScale(this MonoGame.Extended.Tiled.TiledMap t) =>
             new System.Numerics.Vector2(1f / t.TileWidth, 1f / t.TileHeight);

        public static Microsoft.Xna.Framework.Color ToXna(this Box2DSharp.Common.Color c) =>
            new Microsoft.Xna.Framework.Color(c.R, c.G, c.B, c.A);

        public static void LerpToPosition(this MonoGame.Extended.OrthographicCamera camera, Microsoft.Xna.Framework.Vector2 position)
        {
            Microsoft.Xna.Framework.Vector2 delta = position - camera.BoundingRectangle.Center.ToNumerics();

            if (camera.BoundingRectangle.Contains(position))
            {
                delta *= 0.1f;
            }
            camera.Move(delta);
        }
        public static void ClampWithinBounds(this MonoGame.Extended.OrthographicCamera camera, MonoGame.Extended.RectangleF bounds)
        {
            Microsoft.Xna.Framework.Vector2 d = new();
            if (camera.BoundingRectangle.Width > bounds.Width)
            {
                d.X = (bounds.Center.X - camera.BoundingRectangle.Center.X);
            }
            else
            {
                if (camera.BoundingRectangle.Left < bounds.Left)
                {
                    d.X = (bounds.Left - camera.BoundingRectangle.Left);
                }
                if (camera.BoundingRectangle.Right > bounds.Right)
                {
                    d.X = (bounds.Right - camera.BoundingRectangle.Right);
                }
            }
            if (camera.BoundingRectangle.Height > bounds.Height)
            {
                d.Y = (bounds.Center.Y - camera.BoundingRectangle.Center.Y);
            }
            else
            {
                if (camera.BoundingRectangle.Top < bounds.Top)
                {
                    d.Y = (bounds.Top - camera.BoundingRectangle.Top);
                }
                if (camera.BoundingRectangle.Bottom > bounds.Bottom)
                {
                    d.Y = (bounds.Bottom - camera.BoundingRectangle.Bottom);
                }
            }
            camera.Move(d);
        }
    }
}
