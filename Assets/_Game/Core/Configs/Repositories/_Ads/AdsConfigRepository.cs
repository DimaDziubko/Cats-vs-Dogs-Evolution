using _Game.Core.Configs.Models;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.Configs.Repositories._Ads
{
    public class AdsConfigRepository : IAdsConfigRepository
    {
        private readonly IUserContainer _userContainer;

        public AdsConfigRepository(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }

        public AdsConfig GetConfig() => 
            _userContainer.GameConfig.AdsConfig;
    }
}