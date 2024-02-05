using Cysharp.Threading.Tasks;
using DG.Tweening;
using Engine.EnemyComp.Interfaces;
using Engine.EnemyComp.Models;
using Engine.UnitComp;
using Engine.UnitComp.Consts;
using Engine.UnitComp.Interfaces;
using System;
using UnityEngine;

namespace Engine.EnemyComp
{
    public class EnemyUnit : Unit, IEnemyUnit
    {
        public EnemyData EnemyData => _enemyData;

        [SerializeField]
        private EnemyData _enemyData;

        private Tween _hitScaleUpAnimation;

        protected override void Awake()
        {
            base.Awake();

            base.UnitHealth.SetBaseHealth(_enemyData.BaseHealth);
            base.UnitHealth.OnHealthChange += OnHealthChanged;

            base.UnitDeath.OnUnitKilled += OnUnitKilled;
        }

        private void OnUnitKilled(IUnit unit)
        {
            UnitAnimation.ForcePlay(UnitConsts.Anims.DEATH);
            transform.DOScale(Vector3.zero, .25f).SetDelay(.5f);
        }

        private void OnHealthChanged(UnitHealth.HealthChangeData data)
        {
            if (data.CurrentHealth <= 0)
            {
                UnitDeath.KillUnit();
            }
        }

        public override async UniTask HitUnit(IUnit attacker, int damage)
        {
            // WE PUSH BACK ENEMY.
            Vector2 direction = (attacker.RigidBody.position - RigidBody.position).normalized;
            _rigidBody.velocity = Vector2.zero;
            _rigidBody.AddForce(-direction, ForceMode2D.Impulse);

            // HIT SCALE UP ANIMATION.
            _hitScaleUpAnimation?.Kill(true);
            _hitScaleUpAnimation = transform.DOPunchScale(Vector3.one * .2f, .25f, 1);

            base.UnitHealth.ChangeHealth(-damage);

            await UnitAnimation.PlayAnimAsync(UnitConsts.Anims.HURT);

            if (UnitDeath.IsDeath)
                return;

            UnitAnimation.PlayAnim(UnitConsts.Anims.IDLE);
            await UniTask.Yield();
        }

    }
}