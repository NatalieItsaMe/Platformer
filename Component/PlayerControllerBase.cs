namespace Platformer.Component
{
    public class PlayerControllerBase
    {
        public float HorizontalMovementForce { get; set; } = 70f;
        public float MaxHorizontalSpeed { get; set; } = 4.2f;
        public float JumpForce { get; set; } = -20f;
        public ushort MaxJumpTimeout { get; set; } = 4;
        public ushort MaxZoomTimeout { get; set; } = 6;
    }
}