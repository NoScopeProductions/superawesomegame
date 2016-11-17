using UnityEngine;
using ShooterGame.Constants;
using ShooterGame.Interfaces;
using ShooterGame.Managers;
using ShooterGame.Player.StatusEffects;
using System.Collections.Generic;

namespace ShooterGame.Player
{
    public class PlayerStats : ADestructible
    {
        public delegate void PlayerStatsEvent(PlayerStats playerStats);

        public event PlayerStatsEvent OnDie = p => { };
        //public event PlayerStatsEvent OnTakeDamage = p => { };
        //public event PlayerStatsEvent OnHeal = p => { };

        [SerializeField] private Stat _shields;

        private HudManager _hudManager;

        private readonly List<StatusEffectBase> _statusEffects = new List<StatusEffectBase>();
        
        public Stat   Shields         { get { return this._shields;   } }
        private Weapon PrimaryWeapon { get; set; }
        private Weapon SecondaryWeapon { get; set; }

        void Awake()
        {
        }

        void Start()
        {
            GameManager.Instance.OnTurnUpdate += this.TurnUpdate;
            this._hudManager = HudManager.Instance;
        }

        void OnDestroy()
        {
            GameManager.Instance.OnTurnUpdate -= this.TurnUpdate;
        }

        void Update()
        {
            this.HandleTestInputs();
            this._hudManager.ShowLoadout(this.PrimaryWeapon, this.SecondaryWeapon);
        }

        //todo - remove once turn and pvp mechanics are functional
        void HandleTestInputs()
        {
            if (Input.GetButtonDown(Inputs.Temp_TakeDamage))
                this.TakeDamage(10, this, this.transform.position);

            if (Input.GetButtonDown(Inputs.Temp_Heal))
                this.Heal(10, this);

            if (Input.GetButtonDown(Inputs.Temp_RecoverShields))
                this.RecoverShields(10);
        }

        private void TurnUpdate()
        {
            this._statusEffects.ForEach(se => se.ApplyStatusEffect());
            this._statusEffects.RemoveAll(se => se.Duration == 0);
        }

        public void Equip(Weapon weapon)
        {
            this.PrimaryWeapon = weapon;
        }

        public void EquipSecondary(Weapon weapon)
        {
            this.SecondaryWeapon = weapon;
        }

        public void AddStatusEffect(StatusEffectBase statusEffect)
        {
            this._statusEffects.Add(statusEffect);
        }

        void AttackWithPrimary(IDestructible target)
        {
            this.PrimaryWeapon.Attack(target, this);
        }

        void AttackWithSecondary(IDestructible target)
        {
            this.SecondaryWeapon.Attack(target, this);
        }

        #region IDestructible implementation
        public override void TakeDamage(float amount, PlayerStats attacker, Vector2 pointOfContact)
        {
            Debug.Log("Taking Damage");
            float damageAfterShields = this._shields.Value - amount;

            if(this._shields.Value > 0)
                this._shields.Value -= amount;

            if(damageAfterShields < 0)
                this._health.Value += damageAfterShields;

            if (this._health.Value <= 0f)
                this.Die();
        }
        #endregion

        public void Heal(float amount, PlayerStats healer)
        {
            this._health.Value += amount;
        }

        public void RecoverShields(float amount)
        {
            this._shields.Value += amount;
        }

        private void Die()
        {
            Debug.Log("He's dead, Jim.");
            this.OnDie(this);
            Destroy(this.gameObject);
        }
    }
}