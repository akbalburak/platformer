using Cysharp.Threading.Tasks;
using Engine.UnitComp.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.UnitComp
{
    public class UnitAnimation : IUnitAnimation
    {
        public static IUnitAnimation Create(IUnit owner, Animator animator)
        {
            return new UnitAnimation(owner, animator);
        }

        [SerializeField]
        private Animator _animator;

        private bool _animationLocked;

        private AnimatorStateHandler _animatorState;

        private Dictionary<int, AnimatorStateInfo> _animationAndStates
            = new Dictionary<int, AnimatorStateInfo>();

        private IUnit _owner;

        private UnitAnimation(IUnit owner,Animator animator)
        {
            _owner = owner;
            _animator = animator;

            _animatorState = _animator.GetBehaviour<AnimatorStateHandler>();
            _animatorState.OnAnimatorStateEnter += OnAnimatorStateEnter;

        }

        private void OnAnimatorStateEnter(AnimatorStateInfo info)
        {
            if (_animationAndStates.ContainsKey(info.shortNameHash))
                return;

            _animationAndStates.Add(info.shortNameHash, info);
        }

        public void PlayAnim(string state)
        {
            if (_animationLocked)
                return;

            _animator.Play(state);
        }

        public async UniTask PlayAnimAsync(string state)
        {
            if (_animationLocked)
                return;

            _animationLocked = true;
            
            _animator.Play(state);

            int stateHash = Animator.StringToHash(state);
            await UniTask.WaitUntil(() => _animationAndStates.ContainsKey(stateHash), PlayerLoopTiming.Update);

            _animationAndStates.TryGetValue(stateHash, out AnimatorStateInfo info);
            await UniTask.WaitForSeconds(info.length);

            _animationLocked = false;
        }

        public void ForcePlay(string state)
        {
            _animator.Play(state);
        }
    }
}
