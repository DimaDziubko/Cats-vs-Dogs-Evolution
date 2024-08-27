using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Data.Timeline.Static;
using _Game.Utils;
using Assets._Game.Core.Data.Timeline.Static;
using Assets._Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.Timeline
{
    public class TimelineDataProvider :  ITimelineDataProvider
    {
        private readonly ITimelineConfigRepository _timelineConfigRepository;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;

        public TimelineDataProvider(
            IConfigRepositoryFacade configRepositoryFacade,
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _timelineConfigRepository = configRepositoryFacade.TimelineConfigRepository;
            _assetRegistry = assetRegistry;
            _logger = logger;
        }
        
        public async UniTask<TimelineStaticData> Load(int timelineId)
        {
            var infoItemTask = LoadTimelineInfoItems(timelineId);
            
            var results = await UniTask.WhenAll(
                infoItemTask);
            
            TimelineStaticData data = new TimelineStaticData()
            {
                TimelineInfoItems = results[0]
            };

            return data;
        }

        private async UniTask<Dictionary<int, TimlineInfoItemStaticData>> LoadTimelineInfoItems(int timelineId)
        {
            Dictionary<int, TimlineInfoItemStaticData> data = new Dictionary<int, TimlineInfoItemStaticData>();
            
            var ageConfigs = _timelineConfigRepository.GetAgeConfigs();
            
            int ageIndex = 0;
            
            _logger.Log("Loading age icons");
            
            foreach (var config in ageConfigs)
            {
                await _assetRegistry.Warmup<IList<Sprite>>(config.AgeIconAtlas);
            
                IList<Sprite> atlas = await _assetRegistry.LoadAsset<IList<Sprite>>(
                    config.AgeIconAtlas,
                    timelineId,
                    Constants.CacheContext.TIMELINE);
                
                var icon = atlas.FirstOrDefault(x => x.name == config.AgeIconName);
                
                var model = new TimlineInfoItemStaticData
                {
                    Name = config.Name, 
                    Description = config.Description, 
                    DateRange = config.DateRange,
                    AgeIcon = icon
                };
                
                data.Add(ageIndex, model);
                ageIndex++;
            }

            return data;
        }
    }
}