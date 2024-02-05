using Cysharp.Threading.Tasks;

namespace Engine.UnitComp.Interfaces
{
    public interface IHitableUnit
    {
        UniTask HitUnit(IUnit attacker, int damage);
    }
}
