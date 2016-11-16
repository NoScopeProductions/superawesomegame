using UnityEngine;
using JetBrains.Annotations;
using ShooterGame.Interfaces;
using ShooterGame.Managers;

namespace ShooterGame.Player
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private float _attackPower;
        [SerializeField, UsedImplicitly] private readonly int _cooldownLength;

        private int _cooldown; //# of rounds left on cooldown

        public float AttackPower  { get { return _attackPower;    } }
        public int Cooldown       { get { return _cooldown;       } }
        public int CooldownLength { get { return _cooldownLength; } }

        [UsedImplicitly]
        void Awake()
        {
            GameManager.Instance.OnTurnUpdate += TurnUpdate;
        }

        [UsedImplicitly]
        void OnDestroy()
        {
            GameManager.Instance.OnTurnUpdate -= TurnUpdate;
        }

        protected virtual void TurnUpdate()
        {
            if (_cooldown > 0)
                _cooldown -= 1;
        }

        public virtual void Attack(IDestructible target, PlayerStats wielder)
        {
            Use();
            target.TakeDamage(_attackPower, wielder, wielder.transform.position);
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