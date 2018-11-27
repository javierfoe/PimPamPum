using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
namespace Bang
{
    public class GameController : NetworkBehaviour
    {
        public static CardView CardPrefab
        {
            get; private set;
        }

        [SerializeField] private CardView cardPrefab;
        [SerializeField] private BoardController boardController = null;
        [SerializeField] private Transform playerViews = null;

        [SyncVar] private int maxPlayers;

        private int currentPlayer;
        private static PlayerController[] playerControllers;

        public static PlayerController GetPlayerController(int index)
        {
            return playerControllers[index];
        }

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

        public Card DrawDiscardCard()
        {
            Card res = DrawCard();
            Debug.Log("Draw! Card: Suit - " + res.Suit + " Rank - " + res.Rank);
            DiscardCard(res);
            return res;
        }

        public Card DrawCard()
        {
            return boardController.DrawCard();
        }

        public List<Card> DrawCards(int cards)
        {
            return boardController.DrawCards(cards);
        }

        public void DiscardCard(Card card)
        {
            boardController.DiscardCard(card);
        }

        public void AddPlayerControllers(GameObject[] gos)
        {
            int i = 0;
            foreach (GameObject go in gos)
                playerControllers[i++] = go.GetComponent<PlayerController>();

            RpcAddPlayerControllers(gos);
        }

        public void PassDynamite(int player, Dynamite d)
        {
            int playerAux = player;
            PlayerController pc;
            do
            {
                playerAux++;
                playerAux = playerAux > maxPlayers - 1 ? 0 : playerAux;
                pc = playerControllers[playerAux];
            } while (pc.HasProperty<Dynamite>());
            d.EquipProperty(pc);
        }

        public override void OnStartClient()
        {
            CardPrefab = cardPrefab;
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
                if (sheriff == null)
                {
                    sheriff = players[random];
                    currentPlayer = random;
                }
                players[random].SetRole(r);
                players.RemoveAt(random);
            }
            //Debug.Log("CurrentPlayer: " + currentPlayer);
            sheriff.StartTurn();
        }

        public void EndTurn()
        {
            currentPlayer = currentPlayer < maxPlayers - 1 ? currentPlayer + 1 : 0;
            //Debug.Log("CurrentPlayer: " + currentPlayer);
            playerControllers[currentPlayer].StartTurn();
        }

        public List<int> PlayersInRange(int player, int range)
        {
            List<int> res = new List<int>();
            int add, sub;
            for (int i = 0; i < range && i < maxPlayers; i++)
            {
                add = player + i + 1;
                sub = player - i - 1;
                add = add > maxPlayers - 1 ? add - maxPlayers : add;
                sub = sub < 0 ? maxPlayers + sub : sub;
                if (add == player || sub == player) continue;
                AddToTargetList(res, player, add, range);
                AddToTargetList(res, player, sub, range);
            }
            return res;
        }

        private void AddToTargetList(List<int> list, int attacker, int target, int range)
        {
            if (!list.Contains(target) && target < playerControllers.Length && target > -1 && CheckRangeBetweenPlayers(attacker, target, range)) list.Add(target);
        }

        private bool CheckRangeBetweenPlayers(int attacker, int target, int range)
        {
            int normalDistance = attacker - target;
            if (normalDistance < 0) normalDistance = -normalDistance;
            int reverseDistance = attacker + maxPlayers - target;
            if (reverseDistance < 0) reverseDistance = -reverseDistance;
            int distance = normalDistance < reverseDistance ? normalDistance : reverseDistance;
            distance += playerControllers[target].RangeModifier;
            return distance < range + 1;
        }

        public void StopTargeting()
        {
            foreach (PlayerController pc in playerControllers)
                pc.StopTargeting();
        }

        public void TargetPrison(int player)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            foreach (PlayerController pc in playerControllers)
                if (pc.PlayerNumber != player && pc.Role != ERole.SHERIFF && !pc.HasProperty<Jail>())
                    pc.TargetSetTargetable(conn, true);
        }

        public void TargetAllCards(int player)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            foreach (PlayerController pc in playerControllers)
                pc.SetStealable(conn, true);
        }

        public void TargetSelf(int player)
        {
            PlayerController pc = playerControllers[player];
            pc.TargetSetTargetable(pc.connectionToClient, true);
        }

        public void TargetSelfProperty<T>(int player) where T : Property
        {
            PlayerController pc = playerControllers[player];
            pc.TargetSetTargetable(pc.connectionToClient, !pc.HasProperty<T>());
        }

        public void TargetOthers(int player)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            foreach (PlayerController pc in playerControllers)
                if (pc.PlayerNumber != player)
                    pc.TargetSetTargetable(conn, true);
        }

        public void TargetAllRangeCards(int player, int range)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            List<int> playersInRange = PlayersInRange(player, range);
            foreach (int i in playersInRange)
            {
                playerControllers[i].SetStealable(conn, true);
            }
            playerControllers[player].SetStealable(conn, true);
        }

        public void TargetPlayersRange(int player, int range)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            List<int> playersInRange = PlayersInRange(player, range);
            foreach (int i in playersInRange)
            {
                playerControllers[i].TargetSetTargetable(conn, true);
            }
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
    }
}