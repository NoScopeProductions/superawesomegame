using UnityEngine;
using System.Collections;
using ShooterGame.Interfaces;

namespace ShooterGame.Player
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private float _attackPower;
        [SerializeField] private readonly int _cooldownLength;

        private int _cooldown = 0; //# of rounds left on cooldown

        public float AttackPower  { get { return _attackPower;    } }
        public int Cooldown       { get { return _cooldown;       } }
        public int CooldownLength { get { return _cooldownLength; } }

        protected virtual void TurnUpdate()
        {
            if (_cooldown > 0)
                _cooldown -= 1;
        }

        public virtual bool Use()
        {
            if (_cooldown > 0)
                return false;

            _cooldown = _cooldownLength;
            return true;
        }
    }
}