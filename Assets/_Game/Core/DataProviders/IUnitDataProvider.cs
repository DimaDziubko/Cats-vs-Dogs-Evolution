using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.DataProviders
{
    public interface IUnitDataProvider
    {
        UniTask<UnitData> LoadUnitData(UnitLoadOptions options);
        UniTask<UnitBuilderBtnData> LoadUnitBuilderData(BuilderLoadOptions options);
    }
}