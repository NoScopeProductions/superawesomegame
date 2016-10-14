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

        public void Awake()
        {
            this.NetworkManager.JoinedRoom += this.SpawnPlayer;
        }

        private void SpawnPlayer()
        {
            PhotonNetwork.Instantiate(PrefabNames.PLAYER, Vector3.zero, Quaternion.identity, 0);
        }
    }
}