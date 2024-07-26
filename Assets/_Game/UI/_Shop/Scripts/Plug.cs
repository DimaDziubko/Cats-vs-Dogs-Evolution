using _Game.UI.Factory;
using UnityEngine;

namespace _Game.UI._Shop.Scripts
{
    public class Plug : MonoBehaviour
    {
        public IUIFactory OriginFactory { get; set; }
    
        public void Release()
        {
            OriginFactory.Reclaim(this);
        }
    }
}
