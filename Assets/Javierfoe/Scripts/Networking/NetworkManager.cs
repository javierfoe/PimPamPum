using UnityEngine;
using UnityEngine.Networking;

namespace Bang
{

    public class NetworkManager : UnityEngine.Networking.NetworkManager
    {

        [SerializeField] private GameController gameController;

        private GameObject[] playerControllerGameObjects;
        private PlayerController[] playerControllerComponents;
        private int currentPlayers, maxPlayers;

        public void SetPlayerAmount(float amount)
        {
            maxPlayers = (int)amount;
        }

        public override void OnStartServer()
        {
            playerControllerGameObjects = new GameObject[maxPlayers];
            playerControllerComponents = new PlayerController[maxPlayers];
            currentPlayers = 0;
            gameController.MaxPlayers = maxPlayers;
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            if (currentPlayers == maxPlayers)
            {
                conn.Disconnect();
                return;
            }

            PlayerController playerController = Instantiate(playerPrefab).GetComponent<PlayerController>();
            playerController.PlayerNumber = currentPlayers;
            NetworkServer.AddPlayerForConnection(conn, playerController.gameObject, playerControllerId);

            playerControllerComponents[currentPlayers] = playerController;
            playerControllerGameObjects[currentPlayers++] = playerController.gameObject;

            if (currentPlayers == maxPlayers)
            {
                SetupPlayers();
            }

        }

        private void SetupPlayers()
        {
            gameController.SetMatch(maxPlayers, playerControllerGameObjects);
        }
    }
}