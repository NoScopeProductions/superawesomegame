using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ShooterGame.Constants;
using ShooterGame.Player;

namespace ShooterGame.Managers
{
    public class HUD : MonoBehaviour
    {
        [SerializeField]
        private BarScript _healthBar, _shieldBar;

        public static HUD Instance { get; private set; }

        void Awake()
        {
            if (!Instance)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            var barScripts = GetComponentsInChildren<BarScript>().ToDictionary(bar => bar.BarType);
            _healthBar = barScripts[HUDBarType.Health];
            _shieldBar = barScripts[HUDBarType.Shields];
        }

        public void ShowHealth(Stat health)
        {
            ShowStat(_healthBar, health);
        }

        public void ShowShields(Stat shields)
        {
            ShowStat(_shieldBar, shields);
        }

        private void ShowStat(BarScript statusBar, Stat status)
        {
            statusBar.SetFillAmount(status.Value / status.MaxValue);
        }

        public void ShowLoadout(Weapon primary, Weapon secondary)
        {

        }
    }
}