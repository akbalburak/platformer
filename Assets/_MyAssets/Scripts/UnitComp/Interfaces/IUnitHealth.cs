using System;
using static Engine.UnitComp.UnitHealth;

namespace Engine.UnitComp.Interfaces
{
    public interface IUnitHealth
    {
        event Action<HealthChangeData> OnHealthChange;

        void SetBaseHealth(int baseHealth);
        void ChangeHealth(int changeVal);
    }
}
