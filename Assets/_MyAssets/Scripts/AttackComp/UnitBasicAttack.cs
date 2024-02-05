using Cysharp.Threading.Tasks;
using Engine.AttackComp;
using Engine.EnemyComp;
using Engine.UnitComp.Interfaces;

public class UnitBasicAttack : UnitMeleeAttack
{
    public override async UniTask<bool> TryUse()
    {
        UniTask attackAnimation = _owner.UnitAnimation.PlayAnimAsync(_animationName);
        UniTask delayAttack = UniTask.WaitForSeconds(base._delayAttackFrame / 30);

        await delayAttack;
        
        _hitCollider.enabled = true;

        await attackAnimation;

        _hitCollider.enabled = false;

        Complete();

        return true;
    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        // WE ONLY DETECT IF WE HIT ANY UNIT.
        if (!collision.TryGetComponent(out IHitableUnit unit))
            return;

        unit.HitUnit(_owner, _damage);
    }
}
