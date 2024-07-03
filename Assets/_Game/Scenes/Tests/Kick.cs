using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets._Game.Scenes.Tests
{
    public class Kick : MonoBehaviour
    {
        [Button]
        public void Delete()
        {
            Destroy(this.gameObject, 3);
        }
    }
}
