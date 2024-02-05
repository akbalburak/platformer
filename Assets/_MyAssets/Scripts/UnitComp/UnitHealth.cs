using Engine.UnitComp.Interfaces;
using System;

namespace Engine.UnitComp
{
    public class UnitHealth : IUnitHealth
    {
        public static IUnitHealth Create(IUnit owner)
        {
            return new UnitHealth(owner);
        }

        public event Action<HealthChangeData> OnHealthChange;

        private int _baseHealth;
        private int _currentHealth;
        private IUnit _owner;

        private UnitHealth(IUnit owner)
        {
            _owner = owner;
        }

        public void SetBaseHealth(int baseHealth)
        {
            _baseHealth = baseHealth;
            _currentHealth = _baseHealth;
        }

        public void ChangeHealth(int changeVal)
        {
            int oldHealth = _currentHealth;

            _currentHealth += changeVal;

            if (_currentHealth < 0)
                _currentHealth = 0;

            OnHealthChange?.Invoke(new HealthChangeData(
                _baseHealth,
                oldHealth,
                _currentHealth
            ));
        }

        public readonly struct HealthChangeData
        {
            public HealthChangeData(int baseHealth, int oldHealth, int currentHealth)
            {
                BaseHealth = baseHealth;
                OldHealth = oldHealth;
                CurrentHealth = currentHealth;
            }

            public int BaseHealth { get; }
            public int OldHealth { get; }
            public int CurrentHealth { get; }
            public float Ratio => CurrentHealth / (float)BaseHealth;
        }
    }
}
