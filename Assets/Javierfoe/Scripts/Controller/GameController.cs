using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
namespace Bang
{
    public class GameController : NetworkBehaviour
    {

        public static GameController Instance { get; private set; }

        public CardView cardPrefab;

        [SerializeField] private BoardController boardController = null;
        [SerializeField] private Transform playerViews = null;

        [SyncVar] private int maxPlayers;

        private PlayerController[] playerControllers;

        public int MaxPlayers
        {
            private get { return maxPlayers; }
            set { maxPlayers = value; }
        }

        public Transform PlayerViews
        {
            get { return playerViews; }
            set { playerViews = value; }
        }

        public Card DrawCard()
        {
            return boardController.DrawCard();
        }

        public List<Card> DrawCards(int cards)
        {
            return boardController.DrawCards(cards);
        }

        public void AddPlayerControllers(GameObject[] gos)
        {
            int i = 0;
            foreach (GameObject go in gos)
                playerControllers[i++] = go.GetComponent<PlayerController>();

            RpcAddPlayerControllers(gos);
        }

        public override void OnStartClient()
        {
            Instance = this;
            playerControllers = new PlayerController[maxPlayers];
        }

        public IPlayerView GetPlayerView(int index)
        {

            return playerViews.GetChild(index).GetComponent<IPlayerView>();

        }

        public IPlayerView GetPlayerView(int localPlayer, int remotePlayer)
        {

            int index = remotePlayer - localPlayer;
            if (index < 0) index = MaxPlayers + index;
            return GetPlayerView(index);

        }

        public void StartGame()
        {
            boardController.ConstructorBoard();

            ERole[] roles = Roles.GetRoles(MaxPlayers);
            List<PlayerController> players = new List<PlayerController>();
            foreach (PlayerController pc in playerControllers)
                players.Add(pc);

            int range, random;
            PlayerController sheriff = null;
            foreach (ERole r in roles)
            {
                range = players.Count;
                random = Random.Range(0, range);
                if (sheriff == null) sheriff = players[random];
                players[random].SetRole(r);
                players.RemoveAt(random);
            }
            sheriff.StartTurn();
        }

        public List<int> PlayersInRange(int player, int range)
        {
            List<int> res = new List<int>();
            int add, sub;
            for (int i = 0; i < range; i++)
            {
                add = player + i + 1;
                sub = player - i - 1;
                add = add > maxPlayers - 1 ? add - maxPlayers : add;
                sub = sub < 0 ? maxPlayers + sub : sub;
                if (add == player || sub == player) continue;
                if (!res.Contains(add)) res.Add(add);
                if (!res.Contains(sub)) res.Add(sub);
            }
            return res;
        }

        [ClientRpc]
        private void RpcAddPlayerControllers(GameObject[] gos)
        {

            if (isServer) return;
            Debug.Log("AddPlayerControllers RPC: " + gos.Length);
            int i = 0;
            foreach (GameObject go in gos)
                playerControllers[i++] = go.GetComponent<PlayerController>();

        }

        [Command]
        public void CmdStopTargeting(int player)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            foreach (PlayerController pc in playerControllers)
                pc.TargetStopTargeting(conn);
        }

        [Command]
        public void CmdTargetAllButSheriff(int player)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            foreach (PlayerController pc in playerControllers)
                if (pc.PlayerNumber != player && pc.Role != ERole.SHERIFF)
                    pc.TargetSetTargetable(conn, ECardDropArea.PLAY);
        }

        [Command]
        public void CmdTargetAllCards(int player)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            foreach (PlayerController pc in playerControllers)
                pc.TargetSetStealable(conn, ECardDropArea.PLAY);
        }

        [Command]
        public void CmdTargetOthers(int player)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            foreach (PlayerController pc in playerControllers)
                if (pc.PlayerNumber != player)
                    pc.TargetSetTargetable(conn, ECardDropArea.PLAY);
        }

        [Command]
        public void CmdTargetAllRangeCards(int player, int range)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            List<int> playersInRange = PlayersInRange(player, range);
            foreach (int i in playersInRange)
            {
                playerControllers[i].TargetSetStealable(conn, ECardDropArea.PLAY);
            }
            playerControllers[player].TargetSetStealable(conn, ECardDropArea.PLAY);
        }

        [Command]
        public void CmdTargetPlayersRange(int player, int range)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            List<int> playersInRange = PlayersInRange(player, range);
            foreach (int i in playersInRange)
            {
                playerControllers[i].TargetSetTargetable(conn, ECardDropArea.PLAY);
            }
        }
    }
}