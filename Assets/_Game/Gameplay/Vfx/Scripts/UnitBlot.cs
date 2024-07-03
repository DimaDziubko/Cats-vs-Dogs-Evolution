using UnityEngine;

namespace Assets._Game.Gameplay.Vfx.Scripts
{
    public class UnitBlot : VfxEntity
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private Sprite[] _sprites;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        private Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }
        public void Initialize(Vector3 position)
        {
            var index = Random.Range(0, _sprites.Length);
            _spriteRenderer.sprite = _sprites[index];
            
            Position = position;
        }
    }
}