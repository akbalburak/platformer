using Cysharp.Threading.Tasks;

namespace Engine.UnitComp.Interfaces
{
    public interface IUnitAnimation
    {
        void ForcePlay(string state);
        void PlayAnim(string state);
        UniTask PlayAnimAsync(string state);
    }
}
