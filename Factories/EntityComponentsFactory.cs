using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tiled;
using Newtonsoft.Json;
using Platformer.Component;
using Platformer.Systems;
using System.Diagnostics;
using System.Linq;

namespace Platformer.Factories
{
    internal class EntityComponentsFactory(World world, PhysicsSystem physicsSystem, Game game)
    {
        public readonly TiledBodyFactory bodyFactory = new(physicsSystem);
        public readonly SpriteFactory spriteFactory = new(game.Content);

        public void BuildEntitiesFromTiledMap(TiledMap tiledMap)
        {
            bodyFactory.SetTiledMap(tiledMap);
            foreach (var mapObject in tiledMap.ObjectLayers.SelectMany(l => l.Objects))
            {
                Entity entity = world.CreateEntity();
                if (mapObject is TiledMapTileObject tileObject && tileObject.Tile != null)
                {
                    var body = bodyFactory.BuildBodyFromTileObject(tileObject);
                    body.Tag = entity.Id;
                    entity.Attach(body);
                    entity.Attach(spriteFactory.BuildSprite(tileObject));
                    BuildComponentsFromProperties(entity, tileObject.Tile.Properties);
                }
                else
                {
                    var body = bodyFactory.BuildBodyFromMapObject(mapObject);
                    body.Tag = entity.Id;
                    entity.Attach(body);
                    BuildComponentsFromProperties(entity, mapObject.Properties);
                }
            }
        }

        private void BuildComponentsFromProperties(Entity entity, TiledMapProperties properties)
        {
            foreach (var prop in properties)
            {
                switch (prop.Key)
                {
                    case nameof(CameraTarget):
                        entity.Attach(JsonConvert.DeserializeObject<CameraTarget>(prop.Value));
                        break;
                    case nameof(KeyboardController):
                        entity.Attach(JsonConvert.DeserializeObject<KeyboardController>(prop.Value));
                        break;
                    case nameof(DebugController):
                        entity.Attach(JsonConvert.DeserializeObject<DebugController>(prop.Value));
                        break;
                    case nameof(OneWayPlatform):
                        entity.Attach(JsonConvert.DeserializeObject<OneWayPlatform>(prop.Value));
                        break;
                    case nameof(SpringComponent):
                        entity.Attach(JsonConvert.DeserializeObject<SpringComponent>(prop.Value));
                        break;
                    case nameof(AnimatedSprite):
                        entity.Attach(spriteFactory.BuildAnimatedSprite(prop.Value));
                        break;
                    default:
                        Debug.WriteLine($"No such component: {prop.Key}");
                        break;
                }
            }
        }
    }
}
