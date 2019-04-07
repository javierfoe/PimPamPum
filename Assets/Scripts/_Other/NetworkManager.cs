using UnityEngine;
using Mirror;

namespace PimPamPum
{

    public class NetworkManager : Mirror.NetworkManager
    {

        [SerializeField] private GameController gameController = null;

        private int currentPlayers, maxPlayers;

        public void SetPlayerAmount(float amount)
        {
            maxPlayers = (int)amount;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            currentPlayers = 0;
            gameController.MaxPlayers = maxPlayers;
            gameController.AvailableCharacter = spawnPrefabs.Count;
        }

        public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage)
        {
            if (currentPlayers == maxPlayers)
            {
                conn.Disconnect();
                return;
            }

            int character = gameController.AvailableCharacter;
            PlayerController playerController = Instantiate(spawnPrefabs[character]).GetComponent<PlayerController>();
            playerController.PlayerNumber = currentPlayers;
            NetworkServer.AddPlayerForConnection(conn, playerController.gameObject);
            gameController.AddPlayerController(currentPlayers++, playerController);

            if (currentPlayers == maxPlayers)
            {
                gameController.SetMatch();
            }
        }

    }
}