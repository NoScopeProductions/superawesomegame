using JetBrains.Annotations;
using ShooterGame.Interfaces;
using ShooterGame.Managers;
using UnityEngine;

namespace ShooterGame.Player.Stats
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private float _attackPower;
        [SerializeField, UsedImplicitly] private readonly int _cooldownLength;

        private int _cooldown; //# of rounds left on cooldown

        public float AttackPower  { get { return this._attackPower;    } }
        public int Cooldown       { get { return this._cooldown;       } }
        public int CooldownLength { get { return this._cooldownLength; } }

        private void Awake()
        {
            GameManager.Instance.OnTurnUpdate += this.TurnUpdate;
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnTurnUpdate -= this.TurnUpdate;
        }

        protected virtual void TurnUpdate()
        {
            if (this._cooldown > 0)
                this._cooldown -= 1;
        }

        public virtual void Attack(IDestructible target, PlayerStats wielder)
        {
            this.Use();
            target.TakeDamage(this._attackPower, wielder, wielder.transform.position);
        }

        public virtual bool Use()
        {
            if (this._cooldown > 0)
                return false;

            this._cooldown = this._cooldownLength;
            return true;
        }
    }
}