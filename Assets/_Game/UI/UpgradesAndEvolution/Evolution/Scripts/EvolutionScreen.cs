﻿using System;
using _Game.Core._DataPresenters.Evolution;
using _Game.Core._UpgradesChecker;
using _Game.Core.DataPresenters.TimelineTravel;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.UI._MainMenu.Scripts;
using _Game.UI._Shop._MiniShop.Scripts;
using _Game.UI.Header.Scripts;
using _Game.UI.TimelineInfoScreen.Scripts;
using Assets._Game.UI.Common.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.UpgradesAndEvolution.Evolution.Scripts
{
    public class EvolutionScreen : MonoBehaviour, IGameScreen
    {
        [SerializeField] private Canvas _canvas;

        [SerializeField] private EvolutionTab _evolutionTab;
        [SerializeField] private TravelTab _travelTab;
        [SerializeField] private Button _timelineInfoButton;

        private IDisposable _disposable;

        private IEvolutionPresenter _evolutionPresenter;
        private ITimelineTravelPresenter _timelineTravelPresenter;
        private ITimelineInfoScreenProvider _timelineInfoProvider;
        private IAudioService _audioService;
        private IHeader _header;
        private IUpgradesAvailabilityChecker _upgradesChecker;
        public GameScreen GameScreen => GameScreen.Evolution;

        public void Construct(
            IHeader header,
            IEvolutionPresenter evolutionPresenter,
            IAudioService audioService,
            ITimelineInfoScreenProvider timelineInfoScreenProvider,
            ITimelineTravelPresenter timelineTravelPresenter,
            IUpgradesAvailabilityChecker upgradesChecker,
            IWorldCameraService cameraService,
            IMiniShopProvider miniShopProvider)
        {
            _evolutionPresenter = evolutionPresenter;
            _timelineInfoProvider = timelineInfoScreenProvider;
            _timelineTravelPresenter = timelineTravelPresenter;
            _audioService = audioService;
            _header = header;
            _upgradesChecker = upgradesChecker;
            _evolutionTab.Construct(
                evolutionPresenter, 
                audioService, 
                timelineInfoScreenProvider,
                miniShopProvider);
            _travelTab.Construct(
                timelineTravelPresenter,
                audioService,
                timelineInfoScreenProvider,
                cameraService);
        }


        public void Show()
        {
            _header.ShowScreenName(GameScreen.ToString(), Color.white);
            
            _canvas.enabled = true;

            if (IsTimeToTravel())
            {
                _travelTab.Show();
            }
            else
            {
                _evolutionTab.Show();
            }

            Unsubscribe();
            Subscribe();

            _upgradesChecker.MarkAsReviewed(GameScreen);
            _upgradesChecker.MarkAsReviewed(GameScreen.UpgradesAndEvolution);
        }

        public void Hide()
        {
            _canvas.enabled = false;
            
            _travelTab.Hide();
            _evolutionTab.Hide();

            _disposable?.Dispose();

            Unsubscribe();
        }

        private void Subscribe()
        {
            _evolutionPresenter.LastAgeOpened += OnLastAgeOpened;
            _timelineInfoButton.onClick.AddListener(OnTimelineInfoBtnClicked);
        }

        private void Unsubscribe()
        {
            _evolutionPresenter.LastAgeOpened -= OnLastAgeOpened;
             _timelineInfoButton.onClick.RemoveAllListeners();
        }

        private void OnLastAgeOpened()
        {
            _travelTab.Show();
            _evolutionTab.Hide();
        }

        private bool IsTimeToTravel() => _timelineTravelPresenter.IsTimeToTravel();

        private async void OnTimelineInfoBtnClicked()
        {
            _audioService.PlayButtonSound();
            
            var window = await _timelineInfoProvider.Load();
            var isExited = await window.Value.ShowScreen();
            if(isExited) window.Dispose();
            _disposable = window;
        }
    }
}