using Assets._Game.Core.Configs.Models;
using Newtonsoft.Json;

namespace Assets._Game.Utils.Extensions
{
    public static class GameConfigExtensions
    {
        public static string ToJsonString(this GameConfig gameConfig)
        {
            return JsonConvert.SerializeObject(gameConfig, Formatting.Indented);
        }
    }
}