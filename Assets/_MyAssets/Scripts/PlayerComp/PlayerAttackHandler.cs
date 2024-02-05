using Engine.AttackComp;
using Engine.PlayerComp.Interfaces;
using UnityEngine;

namespace Engine.PlayerComp
{
    public class PlayerAttackHandler : MonoBehaviour, IPlayerAttackHandler
    {
        public bool CanAttack { get; private set; }
        public bool IsAttacking { get; private set; }

        [SerializeField]
        private UnitAttack[] _activeAttacks;

        private IPlayerUnit _player;

        private void Start()
        {
            _player = GetComponent<IPlayerUnit>();

            foreach (var attack in _activeAttacks)
            {
                attack.SetOwner(_player);
            }
        }

        private async void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {

                if (!_player.Movement.CanDoAnyAction)
                    return;

                foreach (UnitAttack attack in _activeAttacks)
                {
                    if (!attack.CanUse())
                        continue;

                    _player.Movement.ChangeCanDoActionState(newState: false);
                    IsAttacking = true;
                    await attack.TryUse();
                    IsAttacking = false;
                    _player.Movement.ChangeCanDoActionState(newState: true);
                }
            }
        }

        public void ChangeCanAttackState(bool newState)
        {
            CanAttack = newState;
        }
    }
}
