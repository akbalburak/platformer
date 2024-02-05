using UnityEngine;

namespace Engine.AttackComp
{
    public abstract class UnitMeleeAttack : UnitAttack
    {
        protected Collider2D _hitCollider;

        public override void Start()
        {
            _hitCollider = GetComponent<Collider2D>();
            base.Start();
        }
    }
}
