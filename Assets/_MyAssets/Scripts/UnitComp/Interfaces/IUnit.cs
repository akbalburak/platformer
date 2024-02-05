using UnityEngine;

namespace Engine.UnitComp.Interfaces
{
    public interface IUnit
    {
        SpriteRenderer Renderer { get; }
        Bounds SpriteBounds { get; }
        Rigidbody2D RigidBody { get; }
        IUnitAnimation UnitAnimation { get; }
        IUnitHealth UnitHealth { get; }
        IUnitDeath UnitDeath { get; }
    }
}
