using _Game.Utils._LocalConfigSaver;

namespace _Game.Core.Configs.Providers
{
    public class LocalConfigProvider : ILocalConfigProvider
    {
        public string GetConfig()
        {
            return LocalConfigSaver.GetConfig();
        }
    }

    public interface ILocalConfigProvider
    {
        string GetConfig();
    }
}