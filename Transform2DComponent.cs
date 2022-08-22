using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;

namespace Platformer
{
    internal class Transform2DComponent
    {
        Vector2 position = new Vector2(0, 0); //position in Entity World-space
        float rotation = 0; //z-axis rotation in radians
        Vector2 scale = new Vector2 (1, 1); //scalar multipliers for (x, y) axes

    }
}
