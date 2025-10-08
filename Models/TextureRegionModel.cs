namespace Platformer.Models

{
    public class TextureRegionModel
    {
        // Parameters:
        //   texture:
        //     The texture to create the region from.
        //
        //   x:
        //     The top-left x-coordinate of the region within the texture.
        //
        //   y:
        //     The top-left y-coordinate of the region within the texture.
        //
        //   width:
        //     The width, in pixels, of the region.
        //
        //   height:
        //     The height, in pixels, of the region.
        //
        //   isRotated:
        //     A value indicating whether this texture region is rotated 90 degrees clockwise
        //     in the atlas.
        //
        //   originalSize:
        //     The original size of the texture region before trimming.
        //
        //   offset:
        //     The offset between the top-left corner of the original sprite and the top-left
        //     corner of the trimmed sprite.
        //
        //   originNormalized:
        //     The origin point of the texture region, or null if no origin is specified.
        //
        //   name:
        //     The name of the texture region.
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsRotated { get; set; }
        public float[] Origin { get; set; }
    }
}
