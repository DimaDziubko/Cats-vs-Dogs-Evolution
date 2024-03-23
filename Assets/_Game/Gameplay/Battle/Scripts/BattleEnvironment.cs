using UnityEngine;

namespace _Game.Gameplay.Battle.Scripts
{
    public class BattleEnvironment : MonoBehaviour
    {
        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);
    }
}