﻿using _Game.Core.DataPresenters._BaseDataPresenter;
using _Game.Core.DataPresenters.BattlePresenter;
using _Game.Core.DataPresenters.UnitBuilderDataPresenter;
using _Game.Core.DataPresenters.UnitDataPresenter;
using _Game.Core.DataPresenters.WeaponDataPresenter;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class DataPresentersInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindUnitBuilderDataPresenter();
            BindUnitDataPresenter();
            BindWeaponDataPresenter();
            BindTowerDataPresenter();
            BindBattlePresenter();
        }

        private void BindUnitBuilderDataPresenter() => 
            Container
                .BindInterfacesAndSelfTo<UnitBuilderDataPresenter>()
                .AsSingle();

        private void BindUnitDataPresenter() =>
            Container
                .BindInterfacesAndSelfTo<UnitDataPresenter>()
                .AsSingle();

        private void BindWeaponDataPresenter() =>
            Container
                .BindInterfacesAndSelfTo<WeaponDataPresenter>()
                .AsSingle();

        private void BindTowerDataPresenter() =>
            Container
                .BindInterfacesAndSelfTo<BasePresenter>()
                .AsSingle();

        private void BindBattlePresenter() =>
            Container
                .BindInterfacesAndSelfTo<BattlePresenter>()
                .AsSingle();
    }
}