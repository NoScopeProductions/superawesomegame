using UnityEngine;
using JetBrains.Annotations;
using ShooterGame.Constants;
using ShooterGame.Interfaces;
using ShooterGame.Managers;
using ShooterGame.Player.StatusEffects;
using System.Collections.Generic;

namespace ShooterGame.Player
{
    public class PlayerStats : MonoBehaviour, IDestructible
    {
        [SerializeField] private Stat _health, _shields;

        private Weapon _primary, _secondary;
        private HUD _hud;
        private List<StatusEffectBase> _statusEffects;

        public Stat Health            { get { return _health;    } }
        public Weapon PrimaryWeapon   { get { return _primary;   } }
        public Weapon SecondaryWeapon { get { return _secondary; } }

        [UsedImplicitly]
        void Awake()
        {
            _statusEffects = new List<StatusEffectBase>();
        }

        [UsedImplicitly]
        void Start()
        {
            _hud = HUD.Instance;
        }

        [UsedImplicitly]
        void Update()
        {
            HandleTestInputs();
            TurnUpdate(); //will eventually be called once per turn instead of once per frame

            _hud.ShowHealth(_health);
            _hud.ShowShields(_shields);
            _hud.ShowLoadout(_primary, _secondary);
        }

        //todo - remove once turn and pvp mechanics are functional
        void HandleTestInputs()
        {
            if (Input.GetButtonDown(Inputs.Temp_TakeDamage))
                TakeDamage(10, this);

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
            target.TakeDamage(_primary.AttackPower, this);
        }

        void AttackWithSecondary(IDestructible target)
        {
            target.TakeDamage(_primary.AttackPower, this);
        }

        #region IDestructible implementation
        public void TakeDamage(float amount, PlayerStats attacker)
        {
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
            Destroy(gameObject);
        }
    }
}