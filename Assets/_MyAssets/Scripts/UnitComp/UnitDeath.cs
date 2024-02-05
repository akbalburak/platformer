using Engine.UnitComp.Interfaces;
using System;
using UnityEngine;

namespace Engine.UnitComp
{
    public class UnitDeath : IUnitDeath
    {
        public static IUnitDeath Create(IUnit owner)
        {
            return new UnitDeath(owner);
        }

        public event Action<IUnit> OnUnitKilled;
        public event Action<IUnit> OnUnitRevive;

        public bool IsDeath { get; private set; }

        private IUnit _owner;

        private UnitDeath(IUnit owner)
        {
            _owner = owner;
        }

        public void KillUnit()
        {
            if (IsDeath)
                return;

            IsDeath = true;
            OnUnitKilled?.Invoke(_owner);
        }
        public void ReviveUnit()
        {
            if (!IsDeath)
                return;

            IsDeath = false;
            OnUnitRevive?.Invoke(_owner);
        }
    }
}