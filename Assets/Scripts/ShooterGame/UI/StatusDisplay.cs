using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
using ShooterGame.Player;

namespace ShooterGame.UI
{
    public class StatusDisplay : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private BarScript _healthBar, _shieldBar;

        private void ShowStat(BarScript statusBar, Stat status)
        {
            statusBar.SetFillAmount(status.Value / status.MaxValue);
        }

        public void ShowHealth(Stat health)
        {
            ShowStat(_healthBar, health);
        }

        public void ShowShields(Stat shields)
        {
            ShowStat(_shieldBar, shields);
        }
    }
}