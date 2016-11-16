using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using ShooterGame.Constants;
using ShooterGame.Player;
using UnityEngine.UI;

namespace ShooterGame.UI
{
    public class HUD : MonoBehaviour
    {
        private readonly Dictionary<PlayerStats, StatusDisplay> _playerDisplays =
            new Dictionary<PlayerStats, StatusDisplay>();

        [SerializeField] private Text _wind;

        public static HUD Instance { get; private set; }

        [UsedImplicitly]
        void Awake()
        {
            if (!Instance)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        public void ShowLoadout(Weapon primary, Weapon secondary)
        {

        }

        public void ShowWind(Vector2 direction)
        {
            float speed = Mathf.Abs(direction.magnitude);
            _wind.text = string.Format("{0:n1} ({1:n2}, {2:n2})", speed, direction.x, direction.y);
        }

        public void TrackPlayerStatus(PlayerStats player)
        {
            var go = (GameObject)Instantiate(Resources.Load(PrefabNames.STATUS_DISPLAY));
            var statusDisplay = go.GetComponent<StatusDisplay>();
            statusDisplay.AttachToPlayer(player);
            _playerDisplays.Add(player, statusDisplay);

            player.OnDie += DestroyStatusTracker;
        }

        private void DestroyStatusTracker(PlayerStats playerStats)
        {
            Destroy(_playerDisplays[playerStats]);
            _playerDisplays.Remove(playerStats);
        }
    }
}