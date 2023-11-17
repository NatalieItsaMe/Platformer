namespace Platformer
{
    public static class ExtensionMethods
    {
        public static System.Numerics.Vector2 ToNumerics(this MonoGame.Extended.Point2 p) =>
            new System.Numerics.Vector2(p.X, p.Y);
    }
}
