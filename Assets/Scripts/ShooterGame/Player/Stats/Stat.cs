using System;
using JetBrains.Annotations;
using UnityEngine;

namespace ShooterGame.Player.Stats
{
    [Serializable]
    public struct Stat
    {
        [SerializeField, UsedImplicitly] private float _value, _maxValue;

        public float Value
        {
            get { return this._value; }
            set { this._value = Mathf.Clamp(value, 0, this.MaxValue); }
        }

        public float MaxValue { get { return this._maxValue; } }

        public Stat(float value, float maxValue)
        {
            this._value = value;
            this._maxValue = maxValue;
        }
    }
}