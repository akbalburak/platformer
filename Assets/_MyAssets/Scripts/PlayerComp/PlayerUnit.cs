using Cysharp.Threading.Tasks;
using Engine.PlayerComp.Interfaces;
using Engine.PlayerComp.Models;
using Engine.UnitComp;
using Engine.UnitComp.Consts;
using Engine.UnitComp.Interfaces;
using System;
using UnityEngine;

namespace Engine.PlayerComp
{
    public class PlayerUnit : Unit, IPlayerUnit
    {
        public static IPlayerUnit Player { get; private set; }
        public PlayerData PlayerData => _playerData;

        public IPlayerMovement Movement { get; private set; }
        public IPlayerJump Jump { get; private set; }
        public IPlayerAttackHandler AttackHandler { get; private set; }

        [SerializeField]
        private PlayerData _playerData;

        private Vector3 _initialScale;

        protected override void Awake()
        {
            Player = this;

            base.Awake();

            _initialScale = transform.localScale;
            AttackHandler = GetComponent<IPlayerAttackHandler>();
            Jump = GetComponent<IPlayerJump>();
            Movement = GetComponent<IPlayerMovement>();
            Movement.OnMove += OnPlayerMove;

            UnitHealth.SetBaseHealth(_playerData.BaseHealth);
            UnitHealth.OnHealthChange += OnHealthChanged;

            UnitDeath.OnUnitKilled += OnUnitDeath;
            UnitDeath.OnUnitRevive += OnUnitRevive;
        }

        private void OnUnitRevive(IUnit unit)
        {
            AttackHandler.ChangeCanAttackState(newState: true);
            Movement.ChangeCanDoActionState(newState: true);
            UnitHealth.ChangeHealth(_playerData.BaseHealth);
        }
        private void OnUnitDeath(IUnit unit)
        {
            AttackHandler.ChangeCanAttackState(newState: false);
            Movement.ChangeCanDoActionState(newState: false);

            UnitAnimation.ForcePlay(UnitConsts.Anims.DEATH);
        }

        private void OnHealthChanged(UnitHealth.HealthChangeData data)
        {
            if (data.CurrentHealth <= 0)
            {
                UnitDeath.KillUnit();
            }
        }

        private void OnPlayerMove(PlayerMovement.MovementData data)
        {
            float sign = Mathf.Sign(data.Direction.x);
            transform.localScale = new Vector3(sign * _initialScale.x, 1, 1);
        }

        public override async UniTask HitUnit(IUnit attacker, int damage)
        {
            UniTask hurtAnimation = UnitAnimation.PlayAnimAsync(UnitConsts.Anims.HURT);
            UnitHealth.ChangeHealth(-damage);
            await hurtAnimation;
        }
    }
}