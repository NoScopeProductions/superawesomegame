using UnityEngine;
using JetBrains.Annotations;
using ShooterGame.Constants;
using ShooterGame.Interfaces;
using ShooterGame.Managers;

namespace ShooterGame.Player
{
    public class PlayerStats : MonoBehaviour, IDestructible
    {
        [SerializeField]
        private Stat _health, _shields;

        private Weapon _primary, _secondary;
        private HUD _hud;

        public float Health { get { return _health.Value; } }
        public float MaxHealth { get { return _health.MaxValue; } }

        public Weapon PrimaryWeapon { get { return _primary; } }
        public Weapon SecondaryWeapon { get { return _secondary; } }

        [UsedImplicitly]
        void Start()
        {
            _hud = HUD.Instance;

            _hud.ShowHealth(_health);
            _hud.ShowShields(_shields);

            _hud.ShowLoadout(_primary, _secondary);
        }

        [UsedImplicitly]
        void Update()
        {
            if (Input.GetButtonDown(Inputs.Temp_TakeDamage))
                TakeDamage(10, this);

            if (Input.GetButtonDown(Inputs.Temp_Heal))
                Heal(10, this);

            if(Input.GetButtonDown(Inputs.Temp_RecoverShields))
                RecoverShields(10);

            _hud.ShowHealth(_health);
            _hud.ShowShields(_shields);
        }

        public void Equip(Weapon weapon)
        {
            _primary = weapon;
        }

        public void EquipSecondary(Weapon weapon)
        {
            _secondary = weapon;
        }

        #region IDestructible implementation
        public void TakeDamage(float amount, PlayerStats attacker)
        {
            if (_shields.Value > 0)
                _shields.AddValue(-amount);
            else
                _health.AddValue(-amount);

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