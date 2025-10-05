using MonoGame.Extended.Graphics;
using System;
using System.Collections.Generic;

namespace Platformer.Models

{
    public class AnimationModel
    {
        public string Name { get; set; }
        public bool IsLooping { get; set; }
        public bool IsPingPong { get; set; }
        public bool IsReversed { get; set; }
        public Dictionary<string, float> Frames { get; set; }

        public Action<SpriteSheetAnimationBuilder> BuilderAction
        {
            get => builder =>
            {
                builder.IsLooping(IsLooping)
                .IsPingPong(IsPingPong)
                    .IsReversed(IsReversed);

                foreach (var frame in Frames)
                {
                    builder.AddFrame(frame.Key, TimeSpan.FromSeconds(frame.Value));
                }
            };
        }
    }
}
