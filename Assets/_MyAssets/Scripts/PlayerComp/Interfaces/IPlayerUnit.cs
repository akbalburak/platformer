using Engine.PlayerComp.Models;
using Engine.UnitComp.Interfaces;

namespace Engine.PlayerComp.Interfaces
{
    public interface IPlayerUnit : IUnit
    {
        PlayerData PlayerData { get; }
        IPlayerMovement Movement { get; }
        IPlayerJump Jump { get; }
        IPlayerAttackHandler AttackHandler { get; }
    }
}
