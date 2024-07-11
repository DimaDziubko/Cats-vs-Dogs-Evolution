using System.Collections;
using System.Linq;
using Assets._Game.Gameplay._Units.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts.Attack
{
    public class DamageFlashEffect : MonoBehaviour
    {
        private static readonly int FlashColor = Shader.PropertyToID("_FlashColor");
        private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");

        [SerializeField] private Health _health;
        [SerializeField] private float _flashTime = 0.25f;

        [ColorUsage(true, true)]
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField] private SpriteRenderer[] _spriteRenderers;
        [SerializeField] private Material[] _materials;
        [SerializeField] private AnimationCurve _flashSpeedCurve;
        
        private Coroutine _damageFlashCoroutine;
        
        public void Construct()
        {
            _health.Hit += CallDamageFlash;
            
            _materials = new Material[_spriteRenderers.Length];
            
            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                _materials[i] = _spriteRenderers[i].material;
            }

            if (_materials.Any(m => m == null))
            {
                _materials = null;
            }
        }

        public void Reset()
        {
            SetFlashAmount(0);
        }

        public void Cleanup()
        {
            _health.Hit -= CallDamageFlash;
        }

        private void SetFlashColor()
        {
            foreach (var t in _materials)
            {
                t.SetColor(FlashColor, _flashColor);
            }
        }

        private void CallDamageFlash(float _, float __)
        {
            if (!gameObject.activeInHierarchy || _materials == null)
            {
                return;
            }

            if (_damageFlashCoroutine != null)
            {
                StopCoroutine(_damageFlashCoroutine);
            }

            _damageFlashCoroutine = StartCoroutine(DamageFlasher());

        }

        private IEnumerator DamageFlasher()
        {
            SetFlashColor();

            float elapsedTime = 0f;

            while (elapsedTime < _flashTime)
            {
                elapsedTime += Time.deltaTime;

                var currentFlashAmount = Mathf.Lerp(1f, _flashSpeedCurve.Evaluate(elapsedTime), (elapsedTime / _flashTime));
                
                SetFlashAmount(currentFlashAmount);
                
                yield return null;
            }
        }

        private void SetFlashAmount(float amount)
        {
            foreach (var t in _materials)
            {
                t.SetFloat(FlashAmount, amount);
            }
        }
        
        
#if UNITY_EDITOR

        [Button]
        private void ManualInit()
        {
            _health = GetComponent<Health>();

            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            
        }
#endif
    }
}