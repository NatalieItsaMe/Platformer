using Microsoft.Xna.Framework.Input;

namespace Platformer.Component
{
    internal class KeyboardController : Interfaces.IInputMapping
    {
        public Keys Up = Keys.W, 
            Down = Keys.S, 
            Left = Keys.A, 
            Right = Keys.D,
            Jump = Keys.Space, 
            Exit = Keys.Escape,
            ZoomIn = Keys.PageDown,
            ZoomOut = Keys.PageUp;

    }
}
