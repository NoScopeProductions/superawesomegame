using UnityEngine;
using JetBrains.Annotations;
using ShooterGame.Player;

namespace ShooterGame.UI
{
    public class StatusDisplay : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private BarScript _healthBar, _shieldBar;

        private PlayerStats _player;

        [UsedImplicitly]
        void Update()
        {
            TrackPlayer();
            ShowStat(_healthBar, _player.Health);
            ShowStat(_shieldBar, _player.Shields);
        }

        private void ShowStat(BarScript statusBar, Stat status)
        {
            statusBar.SetFillAmount(status.Value / status.MaxValue);
        }

        public void AttachToPlayer(PlayerStats player)
        {
            _player = player;
        }

        private void TrackPlayer()
        {
            transform.position = _player.transform.position + Vector3.up * 1.5f;
        }
    }
}