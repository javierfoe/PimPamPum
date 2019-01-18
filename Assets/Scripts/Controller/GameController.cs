using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Bang
{
    public class GameController : NetworkBehaviour
    {

        private struct Response
        {
            public Decision decision;
            public bool barrelDraw;
            public Card card;
        }

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

        [SerializeField] private GameObject game = null, mainMenu = null;
        [SerializeField] private CardView cardPrefab = null;
        [SerializeField] private PropertyView propertyPrefab = null;
        [SerializeField] private GeneralStoreView generalStoreCardView = null;
        [SerializeField] private BoardController boardController = null;
        [SerializeField] private Transform players = null;
        [SerializeField] private float decisionTime = 0, bangEventTime = 0;

        [SyncVar] private int maxPlayers;

        private IPlayerView[] playerViews;
        private Decision[] decisionsMade;
        private Card[] cardsUsed;
        private int decisionMaker, generalStoreChoice;
        private PlayerController[] playerControllers;
        private List<Card> generalStoreChoices;
        private List<int> availableCharacters;

        public bool PickedCard
        {
            get; set;
        }

        public Card DrawnCard
        {
            get; private set;
        }

        public int CurrentPlayer
        {
            get; private set;
        }

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

        public Decision GetDecision(int player)
        {
            return decisionsMade[player];
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

        public void EnableOthersProperties(int player, bool value)
        {
            for (int i = player + 1; i < maxPlayers; i++)
            {
                playerControllers[i].EnableProperties(value);
            }
            for (int i = 0; i < player; i++)
            {
                playerControllers[i].EnableProperties(value);
            }
        }

        public void CheckDeath(List<Card> list)
        {
            bool listTaken = false;
            for (int i = 0; i < maxPlayers && !listTaken; i++)
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

        public IEnumerator UsedBeer(int player)
        {
            for (int i = player; i < maxPlayers; i++)
            {
                yield return playerControllers[i].UsedBeer();
            }
            for (int i = 0; i < player; i++)
            {
                yield return playerControllers[i].UsedBeer();
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
            else if (murderer != BangConstants.NoOne)
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

        public int MaxPlayers
        {
            get { return maxPlayers; }
            set { maxPlayers = value; }
        }

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

        public IEnumerator DrawEffect(int player)
        {
            Card c = DrawCard();
            yield return DrawEffect(player, c);
        }

        private IEnumerator DrawEffect(int player, Card c)
        {
            PickedCard = false;
            for (int i = player; i < maxPlayers; i++)
            {
                yield return playerControllers[i].DrawEffectTrigger(c);
            }
            for (int i = 0; i < player; i++)
            {
                yield return playerControllers[i].DrawEffectTrigger(c);
            }
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

        public void EquipPropertyTo(int target, Property p)
        {
            p.EquipProperty(playerControllers[target]);
        }

        public IEnumerator CatBalou(int player, int target, Drop drop, int cardIndex)
        {
            PlayerController pc = playerControllers[player];
            PlayerController targetPc = playerControllers[target];

            yield return targetPc.AvoidCard(player, target);

            if (decisionsMade[target] != Decision.Avoid)
            {
                Card c = null;
                switch (drop)
                {
                    case Drop.Hand:
                        if (player == target && cardIndex < pc.DraggedCardIndex) pc.DraggedCardIndex--;
                        c = targetPc.StealCardFromHand(cardIndex);
                        break;
                    case Drop.Properties:
                        c = targetPc.UnequipProperty(cardIndex);
                        break;
                    case Drop.Weapon:
                        c = targetPc.UnequipWeapon();
                        break;
                }
                pc.DiscardCardUsed();
                DiscardCard(c);
                yield return targetPc.StolenBy(player);
            }
            else
            {
                pc.DiscardCardUsed();
                yield return DiscardUsedCard(target);
            }
        }

        public IEnumerator Panic(int player, int target, Drop drop, int cardIndex)
        {

            PlayerController pc = playerControllers[player];
            PlayerController targetPc = playerControllers[target];

            yield return targetPc.AvoidCard(player, target);

            if (decisionsMade[target] != Decision.Avoid)
            {
                Card c = null;
                switch (drop)
                {
                    case Drop.Hand:
                        if (target == player)
                        {
                            c = null;
                        }
                        else
                        {
                            c = targetPc.StealCardFromHand(cardIndex);
                        }
                        break;
                    case Drop.Properties:
                        c = targetPc.UnequipProperty(cardIndex);
                        break;
                    case Drop.Weapon:
                        c = targetPc.UnequipWeapon();
                        break;
                }
                pc.DiscardCardUsed();
                if (c != null) pc.AddCard(c);
                yield return targetPc.StolenBy(player);
            }
            else
            {
                pc.DiscardCardUsed();
                yield return DiscardUsedCard(target);
            }
        }

        public IEnumerator GeneralStore(int player)
        {
            int next = player;
            int players = PlayersAlive;
            generalStoreChoices = boardController.DrawGeneralStoreCards(players);
            while (players > 1)
            {
                yield return GeneralStoreChoice(next);
                yield return GetCardGeneralStore(next, generalStoreChoice);
                next = NextPlayerAlive(next);
                players--;
            }
            yield return GetCardGeneralStore(next, 0);
            boardController.DisableGeneralStore();
        }

        private IEnumerator GetCardGeneralStore(int player, int choice)
        {
            yield return BangEvent(playerControllers[player] + " has chosen the card: " + generalStoreChoices[choice]);
            boardController.RemoveGeneralStoreCard(choice);
            playerControllers[player].AddCard(generalStoreChoices[choice]);
            generalStoreChoices.RemoveAt(choice);
        }

        public IEnumerator StartDuel(int player, int target)
        {
            int next = player;
            int bangsTarget = 0;

            yield return playerControllers[target].AvoidCard(player, target);

            if (decisionsMade[target] != Decision.Avoid)
            {

                yield return BangEvent("Starts the duel between: " + playerControllers[player] + " and " + playerControllers[target]);

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
                    }
                } while (decisionsMade[next] != Decision.TakeHit);

                playerControllers[player].CheckNoCards();
                playerControllers[target].FinishDuelTarget(bangsTarget);

                yield return HitPlayer(player, next);
            }
            else
            {
                yield return DiscardUsedCard(target);
            }
        }

        public IEnumerator HitPlayer(int player, int target)
        {
            yield return playerControllers[target].Hit(player);
        }

        public void StealIfHandNotEmpty(int player, int target)
        {
            PlayerController pc = playerControllers[player];
            PlayerController targetPc = playerControllers[target];
            Card c = null;
            if (targetPc.HasCards)
            {
                c = targetPc.StealCardFromHand();
                pc.AddCard(c);
            }
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
            if (pc.IsDying) yield return pc.Die(player);
            if (player != BangConstants.NoOne) playerControllers[player].DyingFinished();
        }

        public IEnumerator Bang(int player, int target, int misses = 1)
        {
            int dodges;
            List<Response> responses = BarrelDodge(target, out dodges, misses);
            PlayerController targetPc = playerControllers[target];
            decisionsMade[target] = Decision.Pending;
            bool dodge;
            Response r;
            while (dodges < misses && decisionsMade[target] != Decision.TakeHit)
            {
                targetPc.EnableCardsBangResponse();
                decisionsMade[target] = Decision.Pending;
                while(decisionsMade[target] == Decision.Pending)
                {
                    yield return null;
                }
                dodge = decisionsMade[target] == Decision.Dodge;
                dodges += dodge ? 1 : 0;
                r = new Response
                {
                    decision = decisionsMade[target]
                };
                if (dodge)
                {
                    r.card = cardsUsed[target];
                }
                responses.Add(r);
            }
        }

        private List<Response> BarrelDodge(int target, out int dodges, int misses = 1)
        {
            List<Response> res = new List<Response>();
            dodges = 0;
            bool dodge;
            PlayerController pc = playerControllers[target];
            int barrels = pc.Barrels;
            Card c;
            Response r;
            for (int i = 0; i < barrels && dodges < misses; i++)
            {
                c = DrawCard();
                dodge = Barrel.CheckCondition(c);
                r = new Response
                {
                    decision = Decision.Barrel,
                    card = c,
                    barrelDraw = dodge
                };
                res.Add(r);
                dodges += dodge ? 1 : 0;
            }
            return res;
        }

        public IEnumerator BangEvent(string bangEvent)
        {
            yield return new WaitForSeconds(bangEventTime);
            Debug.Log(bangEvent);
        }

        public IEnumerator BangEventPlayedCard(int player, int target, Card card, Drop drop, int cardIndex)
        {
            PlayerController pc = playerControllers[player];
            PlayerController pcTarget = playerControllers[target];
            yield return BangEvent(pc + " Card: " + card + " Target: " + pcTarget + " Drop: " + drop + " CardIndex: " + cardIndex);
        }

        public IEnumerator Indians(int player, Card c)
        {
            RestartDecisions(player, Everyone);

            PlayerController pc;
            for (int i = 0; i < maxPlayers; i++)
            {
                pc = playerControllers[i];
                if (player != i && !pc.IsDead && !pc.Immune(c))
                {
                    playerControllers[i].EnableCardsIndiansResponse();
                }
                else
                {
                    decisionsMade[i] = Decision.Source;
                }
            }

            yield return DecisionTimer(player);

            yield return ResponsesFinished(player, Everyone);
        }

        public IEnumerator AvoidCard(int player, int target)
        {
            RestartDecisions(player, target);
            playerControllers[target].EnableCardsBangResponse();
            yield return DecisionTimer(player);
        }

        public IEnumerator DiscardUsedCard(int target)
        {
            Card c = cardsUsed[target];
            yield return BangEvent(playerControllers[target] + " has avoided the card effect with: " + c);
            DiscardCard(c);
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
                    yield return HitPlayer(player, target);
                    break;
                case Decision.Avoid:
                    yield return CardResponse(target);
                    break;
            }
        }

        private IEnumerator CardResponse(int target)
        {
            PlayerController pc = playerControllers[target];
            int playerNum = pc.PlayerNumber;
            Card used = cardsUsed[playerNum];
            yield return BangEvent(pc + " has avoided the hit with: " + cardsUsed[playerNum]);
            pc.Response();
            DiscardCard(used);
        }

        public IEnumerator Gatling(int player, Card c)
        {
            RestartDecisions(player, Everyone);
            int dodges;
            PlayerController pc;
            List<Response> barrelCards;
            for (int i = 0; i < maxPlayers; i++)
            {
                pc = playerControllers[i];
                if (player != i && !pc.IsDead && !pc.Immune(c))
                {
                    barrelCards = BarrelDodge(i, out dodges);
                    if (dodges < 1)
                    {
                        playerControllers[i].EnableCardsBangResponse();
                    }
                    else if (dodges > 0)
                    {
                        decisionsMade[i] = Decision.Barrel;
                    }
                }
                else
                {
                    decisionsMade[i] = Decision.Source;
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
        }

        private IEnumerator ResponseDuel(int player, int target)
        {
            EnableResponseDuel(target);
            RestartDecisions(player, target);
            yield return DecisionTimer(player);
        }

        public void RestartDecisions(int player, int target)
        {
            cardsUsed = new Card[maxPlayers];
            decisionsMade = new Decision[maxPlayers];
            if (player != BangConstants.NoOne && target != player) decisionsMade[player] = Decision.Source;
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

        public void Saloon()
        {
            for (int i = 0; i < maxPlayers; i++)
            {
                playerControllers[i].HealFromSaloon();
            }
        }

        public void SetMatch(int maxPlayers, GameObject[] playerControllerGOs)
        {
            MaxPlayers = maxPlayers;
            AddPlayerControllers(playerControllerGOs);

            foreach (PlayerController pc in playerControllers)
                foreach (PlayerController pc2 in playerControllers)
                    pc.Setup(pc2.connectionToClient, pc2.PlayerNumber);

            StartGame();
        }

        public IEnumerator DiscardCardEndTurn(Card c, int player)
        {
            PickedCard = false;
            for (int i = player + 1; i < maxPlayers; i++)
            {
                yield return playerControllers[i].EndTurnDiscard(c);
            }
            for (int i = 0; i < player; i++)
            {
                yield return playerControllers[i].EndTurnDiscard(c);
            }
            if (!PickedCard)
            {
                DiscardCard(c);
            }
        }

        public void SetPlayerViews()
        {
            mainMenu.SetActive(false);
            game.SetActive(true);
            playerViews = new IPlayerView[MaxPlayers];
            int j = 0;
            int i = 0;
            foreach (Transform player in players)
            {
                if (ValidPlayer(i))
                {
                    playerViews[j++] = player.GetComponent<IPlayerView>();
                }
                else
                {
                    player.gameObject.SetActive(false);
                }
                i++;
            }
        }

        private bool ValidPlayer(int player)
        {
            bool res = true;
            switch (player)
            {
                case 1:
                    goto case 7;
                case 3:
                    goto case 5;
                case 4:
                    res = MaxPlayers % 2 == 0;
                    break;
                case 5:
                    res = MaxPlayers > 4;
                    break;
                case 7:
                    res = MaxPlayers > 6;
                    break;
            }
            return res;
        }

        private void AddPlayerControllers(GameObject[] gos)
        {
            int i = 0;
            foreach (GameObject go in gos)
                playerControllers[i++] = go.GetComponent<PlayerController>();

            RpcAddPlayerControllers(gos);
        }

        public IEnumerator DiscardCopiesOf<T>(int player, Property p) where T : Property
        {
            for (int i = player; i < maxPlayers; i++)
            {
                yield return playerControllers[i].DiscardCopiesOf<T>(p);
            }
            for (int i = 0; i < player; i++)
            {
                yield return playerControllers[i].DiscardCopiesOf<T>(p);
            }
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
            return playerViews[index];
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
            CurrentPlayer = -1;
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
                    CurrentPlayer = random;
                }
                players[random].SetRole(r);
                players.RemoveAt(random);
            }
            StartTurn(sheriff);
        }

        public void EndTurn()
        {
            int nextPlayer = CurrentPlayer < maxPlayers - 1 ? CurrentPlayer + 1 : 0;
            StartTurn(nextPlayer);
        }

        private void StartTurn(int player)
        {
            if (CurrentPlayer != BangConstants.NoOne) playerControllers[CurrentPlayer].SetTurn(false);
            CurrentPlayer = player;
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

        public void TargetPrison(int player, Card c)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            foreach (PlayerController pc in playerControllers)
                if (pc.Role != Role.Sheriff && !pc.HasProperty<Jail>() && !pc.IsDead && !pc.Immune(c))
                    pc.SetTargetable(conn, true);
        }

        public void TargetAllCards(int player, Card c)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            foreach (PlayerController pc in playerControllers)
                if (!pc.IsDead && !pc.Immune(c))
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

        public void TargetOthers(int player, Card c)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            foreach (PlayerController pc in playerControllers)
                if (pc.PlayerNumber != player && !pc.IsDead && !pc.Immune(c))
                    pc.SetTargetable(conn, true);
        }

        public void TargetAllRangeCards(int player, int range, Card c)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            List<int> playersInRange = PlayersInRange(player, range);
            PlayerController pc;
            foreach (int i in playersInRange)
            {
                pc = playerControllers[i];
                if (!pc.Immune(c))
                {
                    pc.SetStealable(conn, true);
                }
            }
            playerControllers[player].SetStealable(conn, true);
        }

        public void TargetPlayersRange(int player, int range, Card c)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            List<int> playersInRange = PlayersInRange(player, range);
            PlayerController pc;
            foreach (int i in playersInRange)
            {
                pc = playerControllers[i];
                if (!pc.Immune(c))
                {
                    pc.SetTargetable(conn, true);
                }
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

        public void SetPlayerNames(int playerNum)
        {
            if (playerNum < maxPlayers - 1) return;
            foreach (PlayerController pc in playerControllers)
                pc.SetPlayerName();
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
