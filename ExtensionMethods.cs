namespace Platformer
{
    public static class ExtensionMethods
    {
        public static System.Numerics.Vector2 ToNumerics(this MonoGame.Extended.Point2 p) =>
            new System.Numerics.Vector2(p.X, p.Y);
        public static System.Numerics.Vector2 ToNumerics(this MonoGame.Extended.Size2 s) =>
            new System.Numerics.Vector2(s.Width, s.Height);
        public static System.Numerics.Vector2 GetScale(this MonoGame.Extended.Tiled.TiledMap t) =>
             new System.Numerics.Vector2(1f / t.TileWidth, 1f / t.TileHeight);
    }
}
