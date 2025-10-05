namespace Platformer.Models

{
    public class AnimatedSpriteModel
    {
        public string Name { get; set; }
        public string InitialAnimation { get; set; }
        public TextureAtlasModel TextureAtlas { get; set; }
        public AnimationModel[] Animations { get; set; }
    }
}
