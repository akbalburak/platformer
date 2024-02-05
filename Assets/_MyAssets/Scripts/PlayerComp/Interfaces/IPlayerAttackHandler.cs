namespace Engine.PlayerComp.Interfaces
{
    public interface IPlayerAttackHandler
    {
        bool IsAttacking { get; }
        void ChangeCanAttackState(bool newState);
    }
}
