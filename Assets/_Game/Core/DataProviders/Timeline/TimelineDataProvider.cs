using System.Collections.Generic;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories;
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
        
        public TimelineDataProvider(
            ITimelineConfigRepository timelineConfigRepository,
            IAssetRegistry assetRegistry)
        {
            _timelineConfigRepository = timelineConfigRepository;
            _assetRegistry = assetRegistry;
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

            foreach (var config in ageConfigs)
            {
                var icon = await _assetRegistry.LoadAsset<Sprite>(
                    config.AgeIconKey,
                    timelineId,
                    Constants.CacheContext.TIMELINE);
                
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