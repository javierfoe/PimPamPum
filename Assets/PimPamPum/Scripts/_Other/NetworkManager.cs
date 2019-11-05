using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PimPamPum
{
    public class NetworkManager : Mirror.NetworkManager
    {
        public class CreateCharacterMessage : MessageBase
        {
            public int character, role;
        }

        private static readonly Role[] roles = new Role[] { Role.Sheriff, Role.Renegade, Role.Outlaw, Role.Outlaw, Role.Deputy, Role.Outlaw, Role.Deputy, Role.Renegade };
        private List<int> availableCharacters, availableRoles, availableSpots;
        private int currentPlayers, maxPlayers;
        private Dictionary<NetworkConnection, PlayerController> players;

        public int AvailableCharacter
        {
            get
            {
                int index = Random.Range(0, availableCharacters.Count);
                int res = availableCharacters[index];
                availableCharacters.RemoveAt(index);
                return res;
            }
            set
            {
                availableCharacters = new List<int>();
                for (int i = 0; i < value; i++)
                {
                    availableCharacters.Add(i);
                }
            }
        }

        public void SetPlayerAmount(float amount)
        {
            maxPlayers = (int)amount;
            for (int i = 0; i < amount; i++)
            {
                availableRoles.Add(i);
                availableSpots.Add(i);
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            currentPlayers = 0;
            AvailableCharacter = spawnPrefabs.Count;
            availableRoles = new List<int>();
            availableSpots = new List<int>();
            PlayerAmountSlider.AddListener(SetPlayerAmount);
            players = new Dictionary<NetworkConnection, PlayerController>();
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            ClientScene.Ready(conn);
            ClientScene.AddPlayer();
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            PlayerController player = players[conn];
            int role = -1;
            for(int i = 0; i < roles.Length && role < 0; i++)
            {
                role = roles[i] == player.Role ? i : -1;
            }
            availableRoles.Add(role);
            availableCharacters.Add((int)player.Character);
            availableSpots.Add(player.PlayerNumber);
            players.Remove(conn);
            currentPlayers--;
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            if (currentPlayers == maxPlayers)
            {
                conn.Disconnect();
                return;
            }

            int character = AvailableCharacter;
            int random = Random.Range(0, availableRoles.Count);
            Role role = roles[availableRoles[random]];
            availableRoles.RemoveAt(random);

            GameObject player = Instantiate(spawnPrefabs[character]);
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.Role = role;
            playerController.PlayerNumber = availableSpots[0];
            availableSpots.RemoveAt(0);
            NetworkServer.AddPlayerForConnection(conn, playerController.gameObject);
            players.Add(conn, playerController);

            playerController.TargetSetLocalPlayer(conn, maxPlayers);

            if (++currentPlayers == maxPlayers)
            {
                GameController gc = FindObjectOfType<GameController>();
                gc.SetMatch(players.Values.ToArray());
            }
        }
    }
}