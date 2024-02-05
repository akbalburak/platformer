using System;

namespace Engine.UnitComp.Interfaces
{
    public interface IUnitDeath
    {
        event Action<IUnit> OnUnitKilled;
        event Action<IUnit> OnUnitRevive;

        bool IsDeath { get; }

        void KillUnit();
        void ReviveUnit();
    }
}
