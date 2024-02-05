using System;
using static Engine.PlayerComp.PlayerMovement;

namespace Engine.PlayerComp.Interfaces
{
    public interface IPlayerMovement
    {
        bool CanDoAnyAction { get; }

        // WHEN THE PLAYER MOVES IN A DIRECTION.
        event Action<MovementData> OnMove;

        void ChangeCanDoActionState(bool newState);
    }
}
