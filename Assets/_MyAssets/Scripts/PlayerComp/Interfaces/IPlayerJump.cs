using System;

namespace Engine.PlayerComp.Interfaces
{
    public interface IPlayerJump
    {
        event Action OnPlayerJump;

        bool IsJumping { get; }
        bool Grounded { get; }
    }
}
