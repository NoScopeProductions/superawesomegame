using UnityEngine;
using JetBrains.Annotations;
using ShooterGame.Constants;
using ShooterGame.Interfaces;
using ShooterGame.Managers;
using ShooterGame.Player.StatusEffects;
using System.Collections.Generic;
using ShooterGame.UI;

namespace ShooterGame.Player
{
    public class PlayerStats : ADestructible
    {
        public delegate void PlayerStatsEvent(PlayerStats playerStats);

        public PlayerStatsEvent OnDie, OnTakeDamage, OnHeal;

        [SerializeField] private Stat _shields;

        private Weapon _primary, _secondary;
        private HUD _hud;

        private readonly List<StatusEffectBase> _statusEffects = new List<StatusEffectBase>();
        
        public Stat   Shields         { get { return _shields;   } }
        public Weapon PrimaryWeapon   { get { return _primary;   } }
        public Weapon SecondaryWeapon { get { return _secondary; } }

        [UsedImplicitly]
        void Awake()
        {
        }

        [UsedImplicitly]
        void Start()
        {
            GameManager.Instance.OnTurnUpdate += TurnUpdate;
            _hud = HUD.Instance;
        }

        [UsedImplicitly]
        void OnDestroy()
        {
            GameManager.Instance.OnTurnUpdate -= TurnUpdate;
        }

        [UsedImplicitly]
        void Update()
        {
            HandleTestInputs();
            _hud.ShowLoadout(_primary, _secondary);
        }

        //todo - remove once turn and pvp mechanics are functional
        void HandleTestInputs()
        {
            if (Input.GetButtonDown(Inputs.Temp_TakeDamage))
                TakeDamage(10, this, transform.position);

            if (Input.GetButtonDown(Inputs.Temp_Heal))
                Heal(10, this);

            if (Input.GetButtonDown(Inputs.Temp_RecoverShields))
                RecoverShields(10);
        }

        void TurnUpdate()
        {
            _statusEffects.ForEach(se => se.ApplyStatusEffect());
            _statusEffects.RemoveAll(se => se.Duration == 0);
        }

        public void Equip(Weapon weapon)
        {
            _primary = weapon;
        }

        public void EquipSecondary(Weapon weapon)
        {
            _secondary = weapon;
        }

        public void AddStatusEffect(StatusEffectBase statusEffect)
        {
            _statusEffects.Add(statusEffect);
        }

        void AttackWithPrimary(IDestructible target)
        {
            _primary.Attack(target, this);
        }

        void AttackWithSecondary(IDestructible target)
        {
            _secondary.Attack(target, this);
        }

        #region IDestructible implementation
        public override void TakeDamage(float amount, PlayerStats attacker, Vector2 pointOfContact)
        {
            Debug.Log("Taking Damage");
            float damageAfterShields = _shields.Value - amount;

            if(_shields.Value > 0)
                _shields.AddValue(-amount);

            if(damageAfterShields < 0)
                _health.AddValue(damageAfterShields);

            if (_health.Value <= 0f)
                Die();
        }
        #endregion

        public void Heal(float amount, PlayerStats healer)
        {
            _health.AddValue(amount);
        }

        public void RecoverShields(float amount)
        {
            _shields.AddValue(amount);
        }

        private void Die()
        {
            Debug.Log("He's dead, Jim.");
            OnDie(this);
            Destroy(gameObject);
        }
    }
}