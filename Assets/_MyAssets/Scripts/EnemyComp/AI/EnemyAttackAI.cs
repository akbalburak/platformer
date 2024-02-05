using Cysharp.Threading.Tasks;
using Engine.PlayerComp.Interfaces;
using Engine.UnitComp.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Engine.EnemyComp.AI
{
    public class EnemyAttackAI : BaseActionAI
    {
        private RaycastHit2D[] _hitResults;

        private void Start()
        {
            _hitResults = new RaycastHit2D[5];
        }

        public override async UniTask Use(CancellationToken token)
        {
            await UniTask.Yield();

            Vector3 currentPosition = Owner.RigidBody.position;

            float direction = Owner.Renderer.flipX == true ? 1 : -1;

            // IF BOTTOM OF THE ENEMY IS EMPTY.
            int results = Physics2D.RaycastNonAlloc(
                currentPosition,
                Vector2.right * direction,
                _hitResults
            );

            for(int ii = 0; ii < results; ii++)
            {
                if (!_hitResults[ii].transform.TryGetComponent(out IHitableUnit unit))
                    continue;

                if (unit == Owner)
                    continue;

                unit.HitUnit(Owner, 10).Forget();
            }

            Complete();
        }
    }
}
