using UnityEngine;
using JetBrains.Annotations;
using ShooterGame.Player;
using ShooterGame.Player.Stats;

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
            this.TrackPlayer();
            this._healthBar.ShowStat(this._player.Health);
            this._shieldBar.ShowStat(this._player.Shields);
        }

        public void AttachToPlayer(PlayerStats player)
        {
            this._player = player;
        }

        private void TrackPlayer()
        {
            this.transform.position = this._player.transform.position + Vector3.up * 1.5f;
        }
    }
}