using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System;
using System.Collections.Generic;

namespace Platformer
{
    public class MyTiledMapRenderer : TiledMapRenderer
    {
        private List<TiledMapLayer> _backgroundLayers = new List<TiledMapLayer>();
        private List<TiledMapLayer> _foregroundLayers = new List<TiledMapLayer>();
        public MyTiledMapRenderer(GraphicsDevice graphicsDevice, TiledMap map = null) : base(graphicsDevice, map)
        {
            if (map == null)
                return;

            foreach(TiledMapLayer layer in map.Layers)
            {
                if (layer.Type == "Background" || layer.Name.Contains("Background", StringComparison.CurrentCultureIgnoreCase))
                    _backgroundLayers.Add(layer);
                else
                    _foregroundLayers.Add(layer);
            }
        }

        public void DrawBackgroundLayers(Matrix? viewMatrix = null, Matrix? projectionMatrix = null, Effect effect = null, float depth = 0)
        {
            foreach (var layer in _backgroundLayers)
                Draw(layer, viewMatrix, projectionMatrix, effect, depth);
        }

        public void DrawForegroundLayers(Matrix? viewMatrix = null, Matrix? projectionMatrix = null, Effect effect = null, float depth = 0)
        {
            foreach (var layer in _foregroundLayers)
                Draw(layer, viewMatrix, projectionMatrix, effect, depth);
        }
    }
}
