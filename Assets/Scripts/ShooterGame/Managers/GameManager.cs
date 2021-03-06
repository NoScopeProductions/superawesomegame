﻿using ShooterGame.Camera;
using ShooterGame.Constants;
using ShooterGame.Player;
using ShooterGame.Player.Stats;
using ShooterGame.UI;
using UnityEngine;

namespace ShooterGame.Managers
{
    [RequireComponent(typeof(NetworkManager))]
    public class GameManager : MonoBehaviour
    {
        public delegate void TurnUpdateEvent();

        public event TurnUpdateEvent OnTurnUpdate = () => { };

        public static GameManager Instance { get; private set; }

        private NetworkManager _networkManager;
        private NetworkManager NetworkManager
        {
            get { return this._networkManager ?? (this._networkManager = this.GetComponent<NetworkManager>()); }
        }

        private SmoothFollowCamera _mainCamera;
        private SmoothFollowCamera MainCamera
        {
            get { return this._mainCamera ?? (this._mainCamera = UnityEngine.Camera.main.GetComponent<SmoothFollowCamera>()); }
        }

        public void Awake()
        {
            this.NetworkManager.JoinedRoom += this.SpawnPlayer;
            Instance = this;
        }

       private void SpawnPlayer()
        {
            var player = PhotonNetwork.Instantiate(PrefabNames.PLAYER, Vector3.zero, Quaternion.identity, 0);
            HudManager.Instance.TrackPlayerStatus(player.GetComponent<PlayerStats>());

            player.layer = (int) Layer.Player;
            this.MainCamera.Target = player.transform;
        }

        public void BeginTurn(GameObject player)
        {
            this.MainCamera.Target = player.transform;
            this.OnTurnUpdate(); //TODO: Should we send the player whose turn it is as an event parameter?
        }
    }
}