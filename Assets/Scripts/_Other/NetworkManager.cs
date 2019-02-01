using UnityEngine;
using UnityEngine.Networking;

namespace Bang
{

    public class NetworkManager : UnityEngine.Networking.NetworkManager
    {

        private GameController gameController;
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

            gameController = FindObjectOfType<GameController>();
            gameController.MaxPlayers = maxPlayers;
            gameController.AvailableCharacter = spawnPrefabs.Count;
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            if (currentPlayers == maxPlayers)
            {
                conn.Disconnect();
                return;
            }
            int character = gameController.AvailableCharacter;
            PlayerController playerController = Instantiate(spawnPrefabs[character]).GetComponent<PlayerController>();
            playerController.PlayerNumber = currentPlayers;
            NetworkServer.AddPlayerForConnection(conn, playerController.gameObject, playerControllerId);

            playerControllerComponents[currentPlayers] = playerController;
            playerControllerGameObjects[currentPlayers++] = playerController.gameObject;

            if (currentPlayers == maxPlayers)
            {
                gameController.SetMatch(maxPlayers, playerControllerGameObjects);
            }
        }

    }
}