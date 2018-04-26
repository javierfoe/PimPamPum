using UnityEngine;
using UnityEngine.Networking;

namespace Bang
{

    public class NetworkManagerScript : NetworkManager
    {

        [SerializeField] private GameController gameController = null;
        [SerializeField] private int maxPlayers = 0;
        [SerializeField] private Transform players = null;

        private GameObject[] playerControllerGameObjects;
        private PlayerController[] playerControllerComponents;
        private int currentPlayers;

        void Start()
        {
            new CursorIcons();
        }

        public override void OnStartServer()
        {
            playerControllerGameObjects = new GameObject[maxPlayers];
            playerControllerComponents = new PlayerController[maxPlayers];

            gameController.MaxPlayers = maxPlayers;
            currentPlayers = 0;
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {

            PlayerController playerController = Instantiate(playerPrefab).GetComponent<PlayerController>();
            playerController.PlayerNumber = currentPlayers;
            NetworkServer.AddPlayerForConnection(conn, playerController.gameObject, playerControllerId);

            playerControllerComponents[currentPlayers] = playerController;
            playerControllerGameObjects[currentPlayers++] = playerController.gameObject;

            if (currentPlayers == maxPlayers)
                SetupPlayers();

        }

        private void SetupPlayers()
        {
            gameController.AddPlayerControllers(playerControllerGameObjects);

            foreach (PlayerController pc in playerControllerComponents)
                foreach (PlayerController pc2 in playerControllerComponents)
                    pc.TargetSetup(pc2.connectionToClient, pc2.PlayerNumber);

            gameController.StartGame();
        }

    }
}