using Cysharp.Threading.Tasks;
using Engine.UnitComp.Interfaces;
using UnityEngine;

namespace Engine.AttackComp
{
    public abstract class UnitAttack : MonoBehaviour
    {
        [SerializeField]
        protected string _animationName;

        [SerializeField]
        protected float _cooldown;

        [SerializeField]
        protected float _delayAttackFrame;

        [SerializeField]
        protected int _damage;

        protected IUnit _owner;

        private float _lastUseTime;

        public virtual void Start()
        {

        }

        public virtual void SetOwner(IUnit owner)
        {
            _owner = owner;
        }

        public virtual bool CanUse()
        {
            return Time.time - _lastUseTime >= _cooldown;
        }

        public abstract UniTask<bool> TryUse();

        public virtual void Complete()
        {
            _lastUseTime = Time.time;
        }
    }
}
