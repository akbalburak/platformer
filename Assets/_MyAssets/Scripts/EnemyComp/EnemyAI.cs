using Cysharp.Threading.Tasks;
using Engine.EnemyComp.AI;
using Engine.EnemyComp.Interfaces;
using Engine.UnitComp.Interfaces;
using System;
using System.Threading;
using UnityEngine;

namespace Engine.EnemyComp
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField]
        private BaseActionAI[] _actions;

        private IEnemyUnit _owner;

        private CancellationTokenSource _cancellationTokenSource;

        private void Start()
        {
            _owner = GetComponent<EnemyUnit>();
            _owner.UnitDeath.OnUnitKilled += OnUnitKilled;

            foreach (BaseActionAI action in _actions)
            {
                action.Initialize(_owner);
            }

            StartAI().Forget();
        }

        private void OnUnitKilled(IUnit death)
        {
            if (_cancellationTokenSource == null)
                return;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        private async UniTask StartAI()
        {
            while (true)
            {

                _cancellationTokenSource ??= new CancellationTokenSource();
                var cancellationToken = _cancellationTokenSource.Token;

                foreach (BaseActionAI action in _actions)
                {
                    if (!action.CanUse())
                        continue;

                    await action.Use(cancellationToken);
                }

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }
    }
}
