using Cysharp.Threading.Tasks;
using Engine.EnemyComp.Interfaces;
using Engine.UnitComp.Interfaces;
using System.Threading;
using UnityEngine;

namespace Engine.EnemyComp.AI
{
    public abstract class BaseActionAI : MonoBehaviour
    {
        public IEnemyUnit Owner { get; private set; }

        [SerializeField]
        protected float _checkFrequency;

        [SerializeField]
        protected float _cooldown;

        protected float _lastUseTime;

        public virtual void Initialize(IEnemyUnit owner)
        {
            this.Owner = owner;
        }

        public virtual bool CanUse()
        {
            return Time.time - _lastUseTime >= _cooldown;
        }
        public abstract UniTask Use(CancellationToken token);

        public virtual void Complete()
        {
            _lastUseTime = Time.time;
        }
    }
}
