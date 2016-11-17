using JetBrains.Annotations;
using UnityEngine;

namespace ShooterGame.Managers
{
    public class NetworkManager : Photon.PunBehaviour
    {
        private const string LOBBY_VERSION = "v0.1-dev";

        public delegate void NetworkEvent();

        public event NetworkEvent Connected = () => { };
        public event NetworkEvent JoinRoomFailed = () => { };
        public event NetworkEvent JoinedRoom = () => { };

        [SerializeField, UsedImplicitly] private PhotonLogLevel _logLevel = PhotonLogLevel.Informational;
        [SerializeField, UsedImplicitly] private byte _maxPlayersPerRoom = 4;

        public void Awake()
        {

            PhotonNetwork.logLevel = this._logLevel;

            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = true;
        }

        public void Start()
        {
            PhotonNetwork.ConnectUsingSettings(LOBBY_VERSION);
        }

        public override void OnConnectedToMaster()
        {
            this.Connected.Invoke();

            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            this.JoinRoomFailed.Invoke();

            string roomName = string.Format("Room {0}", PhotonNetwork.GetRoomList().Length);
            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = this._maxPlayersPerRoom }, null);
        }

        

        public override void OnJoinedRoom()
        {
            this.JoinedRoom.Invoke();
        }
    }
}