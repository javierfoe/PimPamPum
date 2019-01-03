using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Bang
{
    public class GameController : NetworkBehaviour
    {

        public static GameObject CardPrefab
        {
            get; private set;
        }

        public static GameObject PropertyPrefab
        {
            get; private set;
        }

        public static GameObject GeneralStorePrefab
        {
            get; private set;
        }

        private static readonly int Everyone = -1;

        [SerializeField] private CardView cardPrefab = null;
        [SerializeField] private PropertyView propertyPrefab = null;
        [SerializeField] private GeneralStoreCardView generalStoreCardView = null;
        [SerializeField] private BoardController boardController = null;
        [SerializeField] private Transform playerViews = null;
        [SerializeField] private float decisionTime = 0, bangEventTime = 0;

        [SyncVar] private int maxPlayers;

        private Decision[] decisionsMade;
        private Card[] cardsUsed;
        private int decisionMaker, currentPlayer, generalStoreChoice, dodges;
        private PlayerController[] playerControllers;
        private List<Card> generalStoreChoices;

        public int PlayersAlive
        {
            get
            {
                int res = 0;
                for (int i = 0; i < maxPlayers; i++)
                {
                    res += playerControllers[i].IsDead ? 0 : 1;
                }
                return res;
            }
        }

        public int PlayerStandingAlone
        {
            get
            {
                int res = -1;
                for (int i = 0; i < maxPlayers; i++)
                {
                    res = !playerControllers[i].IsDead ? i : res;
                }
                return res;
            }
        }

        public bool SheriffFoesAlive
        {
            get
            {
                bool foes = false;
                PlayerController pc;
                for (int i = 0; i < maxPlayers && !foes; i++)
                {
                    pc = playerControllers[i];
                    foes = (pc.Role == Role.Outlaw || pc.Role == Role.Renegade) && !pc.IsDead;
                }
                return foes;
            }
        }

        public bool FinalDuel
        {
            get
            {
                return PlayersAlive < 3;
            }
        }

        public float DecisionTime
        {
            get { return decisionTime; }
        }

        private bool AreDecisionsMade
        {
            get
            {
                bool res;
                if (decisionMaker > Everyone)
                {
                    res = decisionsMade[decisionMaker] != Decision.Pending;
                }
                else
                {
                    res = true;
                    for (int i = 0; i < decisionsMade.Length && res; i++)
                    {
                        res &= decisionsMade[i] != Decision.Pending;
                    }
                }
                return res;
            }
        }

        private int NextPlayerAlive(int player)
        {
            PlayerController pc;
            int res = player;
            do
            {
                res++;
                res = res < maxPlayers ? res : 0;
                pc = playerControllers[res];
            } while (pc.IsDead);
            return res;
        }

        private int PreviousPlayerAlive(int player)
        {
            PlayerController pc;
            int res = player;
            do
            {
                res--;
                res = res > -1 ? res : maxPlayers - 1;
                pc = playerControllers[res];
            } while (pc.IsDead);
            return res;
        }

        public void CheckDeath(List<Card> list)
        {
            bool listTaken = false;
            for (int i = 0; i < maxPlayers; i++)
            {
                listTaken = playerControllers[i].CheckDeath(list);
            }
            if (!listTaken)
            {
                foreach (Card c in list)
                {
                    DiscardCard(c);
                }
            }
        }

        public void CheckMurder(int murderer, int killed)
        {
            Role killedRole = playerControllers[killed].Role;
            if (killedRole == Role.Sheriff)
            {
                int alone = PlayerStandingAlone;
                PlayerController alonePc = playerControllers[alone];
                if (PlayersAlive == 1 && alonePc.Role == Role.Renegade)
                {
                    Win(alone);
                }
                else
                {
                    Win(Team.Outlaw);
                }
            }
            else if (!SheriffFoesAlive)
            {
                Win(Team.Law);
            }
            else if (murderer > -1)
            {
                PlayerController pcMurderer = playerControllers[murderer];
                if (killedRole == Role.Outlaw)
                {
                    pcMurderer.Draw(3);
                }
                else if (killedRole == Role.Deputy && pcMurderer.Role == Role.Sheriff)
                {
                    pcMurderer.DiscardAll();
                }
            }
        }

        private void Win(int player)
        {
            playerControllers[player].Win();
            for (int i = 0; i < maxPlayers; i++)
            {
                if (player != i) playerControllers[i].Lose();
            }
        }

        private void Win(Team team)
        {
            PlayerController pc;
            for (int i = 0; i < maxPlayers; i++)
            {
                pc = playerControllers[i];
                if (pc.BelongsToTeam(team))
                {
                    pc.Win();
                }
                else
                {
                    pc.Lose();
                }
            }
        }

        public void ChooseGeneralStoreCard(int choice)
        {
            generalStoreChoice = choice;
        }

        public PlayerController GetPlayerController(int index)
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

        public void MakeDecision(int player, Card card, Decision decision)
        {
            cardsUsed[player] = card;
            decisionsMade[player] = decision;
        }

        public IEnumerator GeneralStore(int player)
        {
            int next = player;
            int players = PlayersAlive;
            generalStoreChoices = boardController.DrawGeneralStoreCards(players);
            while (players > 1)
            {
                yield return GeneralStoreChoice(next);
                GetCardGeneralStore(next, generalStoreChoice);
                next = NextPlayerAlive(next);
                players--;
            }
            GetCardGeneralStore(next, 0);
            boardController.DisableGeneralStore();
        }

        private void GetCardGeneralStore(int player, int choice)
        {
            boardController.RemoveGeneralStoreCard(choice);
            playerControllers[player].AddCard(generalStoreChoices[choice]);
            generalStoreChoices.RemoveAt(choice);
        }

        public IEnumerator StartDuel(int player, int target)
        {
            int next = player;
            int bangsTarget = 0;
            do
            {
                next = next == player ? target : player;
                yield return ResponseDuel(player, next);
                if (decisionsMade[next] == Decision.Avoid)
                {
                    if (next == target)
                    {
                        bangsTarget++;
                    }
                    yield return BangEvent("Player: " + next + " has used a card to avoid the hit: " + cardsUsed[next]);
                }
            } while (decisionsMade[next] != Decision.TakeHit);
            
            playerControllers[player].CheckNoCards();
            playerControllers[target].FinishDuelTarget(bangsTarget);

            yield return playerControllers[next].Hit(player);
        }

        public IEnumerator Dying(int target, int player)
        {
            decisionMaker = target;
            decisionsMade[target] = Decision.Pending;
            PlayerController pc = playerControllers[target];
            float time = 0;
            while (!AreDecisionsMade && time < decisionTime && pc.IsDying)
            {
                time += Time.deltaTime;
                yield return null;
            }
            pc.EnableDieButton(false);
            if (pc.IsDying) pc.Die(player);
            if (player > -1) playerControllers[player].DyingFinished();
        }

        public IEnumerator WaitForBangResponse(int player, int target, int misses = 1)
        {
            if (target == Everyone)
            {
                yield return GatlingResponse(player);
            }
            else
            {
                yield return BarrelDodge(target, misses);
                Decision decision = Decision.Pending;

                if (dodges >= misses)
                {
                    decisionsMade[target] = Decision.Barrel;
                }
                else
                {
                    while (dodges < misses && decision != Decision.TakeHit)
                    {
                        EnableResponse<Missed>(player, target);
                        yield return PlayerDecisions(player, target);
                        decision = decisionsMade[target];
                        dodges += decision == Decision.Avoid ? 1 : 0;
                    }
                }
            }

            yield return ResponsesFinished(player, target);
        }

        private IEnumerator BarrelDodge(int target, int misses = 1)
        {
            dodges = 0;
            Card c;
            bool dodge;
            for (int i = 0; i < playerControllers[target].Barrels && dodges < misses; i++)
            {
                c = DrawDiscardCard();
                dodge = Barrel.CheckCondition(c);
                dodges += dodge ? 1 : 0;
                yield return BangEvent("Player" + target + (dodge ? " has avoided a hit with the barrel." : " the barrel didn't help.") + " Card: " + c);
            }
        }

        public IEnumerator BangEvent(string bangEvent)
        {
            yield return new WaitForSeconds(bangEventTime);
            Debug.Log(bangEvent);
        }

        public IEnumerator WaitForIndiansResponse(int player)
        {
            yield return IndiansResponse(player);
        }

        private IEnumerator ResponsesFinished(int player, int target)
        {
            if (target == Everyone)
            {
                for (int i = player + 1; i < maxPlayers; i++)
                {
                    yield return DecisionConsequence(i, player);
                }

                for (int i = 0; i < player; i++)
                {
                    yield return DecisionConsequence(i, player);
                }
            }
            else
            {
                yield return DecisionConsequence(target, player);
            }
        }

        private IEnumerator DecisionConsequence(int target, int player)
        {
            switch (decisionsMade[target])
            {
                case Decision.TakeHit:
                    yield return playerControllers[target].Hit(player);
                    break;
                case Decision.Avoid:
                    Card used = cardsUsed[target];
                    yield return BangEvent("Player" + target + " has avoided the hit with: " + cardsUsed[target]);
                    DiscardCard(used);
                    break;
            }
        }

        public IEnumerator WaitForGatlingResponse(int player)
        {
            yield return WaitForBangResponse(player, Everyone);
        }

        private IEnumerator GatlingResponse(int player)
        {
            RestartDecisions(player, Everyone);

            PlayerController pc;
            for (int i = 0; i < maxPlayers; i++)
            {
                pc = playerControllers[i];
                yield return BarrelDodge(i);
                if (player != i && !pc.IsDead && dodges < 1)
                {
                    playerControllers[i].EnableCardsResponse<Missed>();
                }
                else if(dodges > 0)
                {
                    decisionsMade[i] = Decision.Barrel;
                }
                else
                {
                    decisionsMade[i] = Decision.Avoid;
                }
            }

            yield return DecisionTimer(player);
        }

        private IEnumerator IndiansResponse(int player)
        {
            RestartDecisions(player, Everyone);

            PlayerController pc;
            for (int i = 0; i < maxPlayers; i++)
            {
                pc = playerControllers[i];
                if (player != i && !pc.IsDead)
                {
                    playerControllers[i].EnableCardsResponse<Bang>();
                }
                else
                {
                    decisionsMade[i] = Decision.Avoid;
                }
            }

            yield return DecisionTimer(player);

            yield return ResponsesFinished(player, Everyone);
        }

        private void EnableResponseDuel(int player)
        {
            playerControllers[player].EnableCardsDuelResponse();
        }

        private IEnumerator GeneralStoreChoice(int player)
        {
            generalStoreChoice = -1;
            float time = 0;
            NetworkConnection conn = playerControllers[player].connectionToClient;
            boardController.EnableCards(conn, true);
            while (generalStoreChoice < 0 && time < decisionTime)
            {
                time += Time.deltaTime;
                yield return null;
            }
            boardController.EnableCards(conn, false);
            generalStoreChoice = generalStoreChoice < 0 ? Random.Range(0, generalStoreChoices.Count) : generalStoreChoice;

            yield return BangEvent("Player: " + player + " has chosen the card: " + generalStoreChoices[generalStoreChoice]);
        }

        private IEnumerator ResponseDuel(int player, int target)
        {
            EnableResponseDuel(target);
            yield return PlayerDecisions(player, target);
        }

        private void EnableResponse<T>(int player, int target) where T : Card
        {
            if (target == Everyone)
            {
                for (int i = 0; i < maxPlayers; i++)
                {
                    if (i != player) playerControllers[i].EnableCardsResponse<T>();
                }
            }
            else
            {
                playerControllers[target].EnableCardsResponse<T>();
            }
        }

        private void RestartDecisions(int player, int target)
        {
            cardsUsed = new Card[maxPlayers];
            decisionsMade = new Decision[maxPlayers];
            if (target != player) decisionsMade[player] = Decision.Source;
            decisionMaker = target;
        }

        private IEnumerator DecisionTimer(int player)
        {
            float time = 0;
            while (!AreDecisionsMade && time < decisionTime)
            {
                time += Time.deltaTime;
                yield return null;
            }

            Decision ed;
            for (int i = 0; i < maxPlayers; i++)
            {
                playerControllers[i].EnableTakeHitButton(false);
                ed = decisionsMade[i];
                decisionsMade[i] = ed == Decision.Pending ? Decision.TakeHit : ed;
            }

            for (int i = 0; i < maxPlayers; i++)
            {
                if (i != player) playerControllers[i].DisableCards();
            }
        }

        private IEnumerator PlayerDecisions(int player, int target)
        {
            RestartDecisions(player, target);
            yield return DecisionTimer(player);
        }

        public void Saloon()
        {
            for (int i = 0; i < maxPlayers; i++)
            {
                playerControllers[i].HealFromSaloon();
            }
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
                playerAux = NextPlayerAlive(playerAux);
                pc = playerControllers[playerAux];
            } while (pc.HasProperty<Dynamite>());
            d.EquipProperty(pc);
        }

        public override void OnStartClient()
        {
            CardPrefab = cardPrefab.gameObject;
            PropertyPrefab = propertyPrefab.gameObject;
            GeneralStorePrefab = generalStoreCardView.gameObject;
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
            currentPlayer = -1;
            Role[] roles = Roles.GetRoles(MaxPlayers);
            List<PlayerController> players = new List<PlayerController>();
            foreach (PlayerController pc in playerControllers)
                players.Add(pc);

            int range, random;
            int sheriff = -1;
            foreach (Role r in roles)
            {
                range = players.Count;
                random = Random.Range(0, range);
                if (sheriff < 0)
                {
                    sheriff = random;
                    currentPlayer = random;
                }
                players[random].SetRole(r);
                players.RemoveAt(random);
            }
            StartTurn(sheriff);
        }

        public void EndTurn()
        {
            int nextPlayer = currentPlayer < maxPlayers - 1 ? currentPlayer + 1 : 0;
            StartTurn(nextPlayer);
        }

        private void StartTurn(int player)
        {
            if (currentPlayer > -1) playerControllers[currentPlayer].SetTurn(false);
            currentPlayer = player;
            playerControllers[player].StartTurn();
        }

        public List<int> PlayersInRange(int player, int range)
        {
            List<int> res = new List<int>();

            TraversePlayers(res, player, range, true);
            TraversePlayers(res, player, range, false);

            return res;
        }

        private void TraversePlayers(List<int> players, int player, int range, bool forward)
        {
            int auxRange = 0;
            int next = player;
            PlayerController pc;
            bool dead;
            do
            {
                next = forward ? NextPlayerAlive(next) : PreviousPlayerAlive(next);
                pc = playerControllers[next];
                dead = pc.IsDead;
                auxRange += dead ? 0 : 1;
                if (!dead && next != player && pc.RangeModifier + auxRange < range + 1 && !players.Contains(next)) players.Add(next);
            } while (next != player);
        }

        public void TargetPrison(int player)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            foreach (PlayerController pc in playerControllers)
                if (pc.PlayerNumber != player && pc.Role != Role.Sheriff && !pc.HasProperty<Jail>() && !pc.IsDead)
                    pc.SetTargetable(conn, true);
        }

        public void TargetAllCards(int player)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            foreach (PlayerController pc in playerControllers)
                if (!pc.IsDead)
                    pc.SetStealable(conn, true);
        }

        public void TargetSelf(int player)
        {
            PlayerController pc = playerControllers[player];
            pc.SetTargetable(pc.connectionToClient, true);
        }

        public void TargetSelfProperty<T>(int player) where T : Property
        {
            PlayerController pc = playerControllers[player];
            pc.SetTargetable(pc.connectionToClient, !pc.HasProperty<T>());
        }

        public void TargetOthers(int player)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            foreach (PlayerController pc in playerControllers)
                if (pc.PlayerNumber != player && !pc.IsDead)
                    pc.SetTargetable(conn, true);
        }

        public void TargetAllRangeCards(int player, int range)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            List<int> playersInRange = PlayersInRange(player, range);
            PlayerController pc;
            foreach (int i in playersInRange)
            {
                pc = playerControllers[i];
                if (!pc.IsDead)
                {
                    pc.SetStealable(conn, true);
                }
            }
            playerControllers[player].SetStealable(conn, true);
        }

        public void TargetPlayersRange(int player, int range)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            List<int> playersInRange = PlayersInRange(player, range);
            foreach (int i in playersInRange)
            {
                playerControllers[i].SetTargetable(conn, true);
            }
        }

        public void StopTargeting(int playerNum)
        {
            NetworkConnection conn = playerControllers[playerNum].connectionToClient;
            foreach (PlayerController pc in playerControllers)
                pc.StopTargeting(conn);
            boardController.SetTargetable(conn, false);
        }

        public void HighlightTrash(int player, bool value)
        {
            boardController.SetTargetable(playerControllers[player].connectionToClient, value);
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
