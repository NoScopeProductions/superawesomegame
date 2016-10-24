using ShooterGame.Camera;
using ShooterGame.Constants;
using ShooterGame.Player;
using UnityEngine;

namespace ShooterGame.Managers
{
    [RequireComponent(typeof(NetworkManager))]
    public class GameManager : MonoBehaviour
    {
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
        }

        private void SpawnPlayer()
        {
            var player = PhotonNetwork.Instantiate(PrefabNames.PLAYER, Vector3.zero, Quaternion.identity, 0);

            this.MainCamera.Target = player.transform;
        }
    }
}