using UnityEngine;
using Mirror;

namespace Bang
{

    public class NetworkManager : Mirror.NetworkManager
    {

        private int currentPlayers, maxPlayers;

        public void SetPlayerAmount(float amount)
        {
            maxPlayers = (int)amount;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            currentPlayers = 0;
        }

        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);
            if (currentPlayers > 0) return;
            GameController.Instance.MaxPlayers = maxPlayers;
            GameController.Instance.AvailableCharacter = spawnPrefabs.Count;
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            if (currentPlayers == maxPlayers)
            {
                conn.Disconnect();
                return;
            }
            int character = GameController.Instance.AvailableCharacter;
            PlayerController playerController = Instantiate(spawnPrefabs[character]).GetComponent<PlayerController>();
            playerController.PlayerNumber = currentPlayers;
            NetworkServer.AddPlayerForConnection(conn, playerController.gameObject);

            GameController.Instance.AddPlayerController(currentPlayers++, playerController);

            if (currentPlayers == maxPlayers)
            {
                GameController.Instance.SetMatch();
            }

        }

    }
}