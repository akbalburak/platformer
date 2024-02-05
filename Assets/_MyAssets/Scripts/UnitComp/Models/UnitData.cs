using System;
using UnityEngine;

namespace Engine.UnitComp.Models
{
    [Serializable]
    public class UnitData
    {
        [SerializeField]
        private int _baseHealth;

        [SerializeField]
        private float _movementSpeed;

        [SerializeField]
        private float _jumpPower;

        [SerializeField]
        private float _slidePower;

        public int BaseHealth => _baseHealth;
        public float MovementSpeed => _movementSpeed;
        public float JumpPower => _jumpPower;
        public float SlidePower => _slidePower;
    }
}
