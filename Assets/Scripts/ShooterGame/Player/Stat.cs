using System;
using JetBrains.Annotations;
using UnityEngine;

namespace ShooterGame.Player
{
    [Serializable]
    public class Stat
    {
        [SerializeField, UsedImplicitly] private float _value, _maxValue;

        public float Value { get { return _value; } }
        public float MaxValue { get { return _maxValue; } }

        public void SetValue(float value)
        {
            _value = Mathf.Clamp(value, 0, MaxValue);
        }

        public void AddValue(float value)
        {
            _value = Mathf.Clamp(_value + value, 0, MaxValue);
        }
    }
}