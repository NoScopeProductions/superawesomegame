﻿using System.Collections.Generic;
using JetBrains.Annotations;
using ShooterGame.Constants;
using ShooterGame.Player;
using ShooterGame.Player.Stats;
using ShooterGame.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ShooterGame.Managers
{
    public class HudManager : MonoBehaviour
    {
        private readonly Dictionary<PlayerStats, StatusDisplay> _playerDisplays =
            new Dictionary<PlayerStats, StatusDisplay>();

        [SerializeField, UsedImplicitly] private Text _wind;

        public static HudManager Instance { get; private set; }

        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else if (Instance != this)
                Destroy(this.gameObject);

            DontDestroyOnLoad(this.gameObject);
        }

        public void ShowLoadout(Weapon primary, Weapon secondary)
        {

        }

        public void ShowWind(Vector2 windForce)
        {
            float speed = Mathf.Abs(windForce.magnitude);
            float angle = Vector2.Angle(windForce.normalized, Vector2.right);
            this._wind.text = string.Format("{0:n1} ({1:n2})", speed, angle);
        }

        public void TrackPlayerStatus(PlayerStats player)
        {
            var go = (GameObject)Instantiate(Resources.Load(PrefabNames.STATUS_DISPLAY));
            var statusDisplay = go.GetComponent<StatusDisplay>();
            statusDisplay.AttachToPlayer(player);
            this._playerDisplays.Add(player, statusDisplay);

            player.OnDie += this.DestroyStatusTracker;
        }

        private void DestroyStatusTracker(PlayerStats playerStats)
        {
            Destroy(this._playerDisplays[playerStats]);
            this._playerDisplays.Remove(playerStats);
        }
    }
}