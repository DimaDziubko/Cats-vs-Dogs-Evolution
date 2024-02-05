using _Game.Core.Services.PersistentData;

namespace _Game.UI.Common.Header.Scripts
{
    public interface IHeader
    {
        void ShowWindowName(string windowName);
        void ShowWallet(IPersistentDataService persistentData);
    }
}
