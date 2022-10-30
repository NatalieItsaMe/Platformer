namespace Platformer.Components
{
    public class ContactFilter
    {
        public bool useTriggers { get; set; } = false;
        public bool useLayerMask { get; set; } = true;
        public byte layerMask { get; internal set; } = (byte)ContactLayers.ENTITY;
    }
}