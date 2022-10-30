using Microsoft.Xna.Framework;

namespace Platformer.Components
{
    internal class RaycastHit
    {
        internal Vector2 normal;
        internal object collider;
        internal float distance;
    }
}