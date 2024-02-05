using Engine.EnemyComp.Models;
using Engine.UnitComp.Interfaces;

namespace Engine.EnemyComp.Interfaces
{
    public interface IEnemyUnit : IUnit
    {
        public EnemyData EnemyData { get; }
    }
}
