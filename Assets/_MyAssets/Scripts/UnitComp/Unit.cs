using Cysharp.Threading.Tasks;
using Engine.UnitComp.Interfaces;
using UnityEngine;

namespace Engine.UnitComp
{
    public abstract class Unit : MonoBehaviour, IUnit, IHitableUnit
    {
        public Rigidbody2D RigidBody => _rigidBody;

        public IUnitAnimation UnitAnimation { get; private set; }
        public IUnitHealth UnitHealth { get; private set; }
        public IUnitDeath UnitDeath { get; private set; }

        public Bounds SpriteBounds => _spriteRenderer.bounds;
        public SpriteRenderer Renderer => _spriteRenderer;

        protected Animator _animator;
        protected Rigidbody2D _rigidBody;
        protected IUnitAnimation _unitAnimation;
        protected SpriteRenderer _spriteRenderer;

        protected virtual void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();

            UnitHealth = UnitComp.UnitHealth.Create(this);
            UnitAnimation = UnitComp.UnitAnimation.Create(this, _animator);
            UnitDeath = UnitComp.UnitDeath.Create(this);
        }

        private void OnPlayerKilled(IUnit unit)
        {
            Debug.Log("Öldü");
        }

        public abstract UniTask HitUnit(IUnit attacker, int damage);
    }
}
