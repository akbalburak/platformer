using Cysharp.Threading.Tasks;
using Engine.UnitComp.Consts;
using System.Threading;
using UnityEngine;

namespace Engine.EnemyComp.AI
{
    public class EnemyMovementAI : BaseActionAI
    {
        private RaycastHit2D[] _hitResults;

        private void Start()
        {
            _hitResults = new RaycastHit2D[2];
        }

        public override async UniTask Use(CancellationToken token)
        {
            float side = Random.Range(0f, 1f) > .5f ? 1 : -1;
            Owner.Renderer.flipX = side == 1;

            float moveSecs = Random.Range(1f, 2f);

            while (moveSecs > 0)
            {
                Owner.UnitAnimation.PlayAnim(UnitConsts.Anims.RUN);

                moveSecs -= Time.fixedDeltaTime;

                // NEXT MOVE POSITION.
                Vector3 currentPosition = Owner.RigidBody.position;
                currentPosition.x += Owner.EnemyData.MovementSpeed * Time.fixedDeltaTime * side;

                Vector2 forward = Vector2.right * side;

                // IF BOTTOM OF THE ENEMY IS EMPTY.
                int results = Physics2D.RaycastNonAlloc(
                    currentPosition,
                    Vector2.down + forward,
                    _hitResults
                );

                if (results <= 1)
                {
                    side *= -1;
                    Owner.Renderer.flipX = side == 1;
                }
                else
                {
                    // MOVE WITH PHYSICS.
                    Owner.RigidBody.MovePosition(currentPosition);
                }

                // WAIT TILL NEXT FRAME.
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
            }

            // PLAY IDLE ANIMATION.
            Owner.UnitAnimation.PlayAnim(UnitConsts.Anims.IDLE);

            Complete();
        }
    }
}
