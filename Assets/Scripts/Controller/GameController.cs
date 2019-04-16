using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace PimPamPum
{
    public class GameController : NetworkBehaviour
    {

        public static GameController Instance
        {
            get; private set;
        }

        [SerializeField] private CardView cardPrefab = null;
        [SerializeField] private PropertyView propertyPrefab = null;
        [SerializeField] private GeneralStoreView generalStoreCardView = null;
        [SerializeField] private BoardController boardController = null;
        [SerializeField] private Transform players = null;
        [SerializeField] private float decisionTime = 0, pimPamPumEventTime = 0;

        private IPlayerView[] playerViews;
        private Decision decision;
        private Card cardUsed;
        private int generalStoreChoice;
        private PlayerController[] playerControllers;
        private List<Card> generalStoreChoices;

        public GameObject CardPrefab
        {
            get { return cardPrefab.gameObject; }
        }

        public GameObject PropertyPrefab
        {
            get { return propertyPrefab.gameObject; }
        }

        public GameObject GeneralStorePrefab
        {
            get { return generalStoreCardView.gameObject; }
        }

        public int MaxPlayers
        {
            get; set;
        }

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
                for (int i = 0; i < MaxPlayers; i++)
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
                for (int i = 0; i < MaxPlayers; i++)
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
                for (int i = 0; i < MaxPlayers && !foes; i++)
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

        private int NextPlayerAlive(int player)
        {
            PlayerController pc;
            int res = player;
            do
            {
                res++;
                res = res < MaxPlayers ? res : 0;
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
                res = res > -1 ? res : MaxPlayers - 1;
                pc = playerControllers[res];
            } while (pc.IsDead);
            return res;
        }

        public void EnableOthersProperties(int player, bool value)
        {
            for (int i = player == MaxPlayers - 1 ? 0 : player + 1; i != player; i = i == MaxPlayers - 1 ? 0 : i + 1)
            {
                playerControllers[i].EnableProperties(value);
            }
        }

        public void CheckDeath(List<Card> list)
        {
            bool listTaken = false;
            for (int i = 0; i < MaxPlayers && !listTaken; i++)
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
            for (int i = player, j = 0; j < MaxPlayers; i = i == MaxPlayers - 1 ? 0 : i + 1, j++)
            {
                yield return playerControllers[i].UsedBeer(player);
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
            else if (murderer != PimPamPumConstants.NoOne)
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
            for (int i = 0; i < MaxPlayers; i++)
            {
                if (player != i) playerControllers[i].Lose();
            }
        }

        private void Win(Team team)
        {
            PlayerController pc;
            for (int i = 0; i < MaxPlayers; i++)
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

        public IEnumerator DrawEffect(int player)
        {
            DrawnCard = DrawCard();
            yield return DrawEffect(player, DrawnCard);
        }

        public IEnumerator DrawEffect(int player, Card c)
        {
            PickedCard = false;
            for (int i = player, j = 0; j < MaxPlayers; i = i == MaxPlayers - 1 ? 0 : i + 1, j++)
            {
                yield return playerControllers[i].DrawEffect(c);
            }
            if (!PickedCard)
            {
                DiscardCard(c);
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
            cardUsed = card;
            this.decision = decision;
        }

        public void EquipPropertyTo(int target, Property p)
        {
            p.EquipProperty(playerControllers[target]);
        }

        public IEnumerator CatBalou(int player, int target, Drop drop, int cardIndex)
        {
            PlayerController pc = playerControllers[player];
            PlayerController targetPc = playerControllers[target];

            decision = Decision.Pending;
            yield return targetPc.AvoidCard(player, target);

            if (decision != Decision.Avoid)
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

            decision = Decision.Pending;
            yield return targetPc.AvoidCard(player, target);

            if (decision != Decision.Avoid)
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
            NetworkConnection conn;
            float time;
            while (players > 1)
            {
                generalStoreChoice = -1;
                time = 0;
                conn = playerControllers[next].connectionToClient;
                boardController.EnableCards(conn, true);
                while (generalStoreChoice < 0 && time < decisionTime)
                {
                    time += Time.deltaTime;
                    yield return null;
                }
                boardController.EnableCards(conn, false);
                generalStoreChoice = generalStoreChoice < 0 ? Random.Range(0, generalStoreChoices.Count) : generalStoreChoice;
                yield return GetCardGeneralStore(next, generalStoreChoice);
                next = NextPlayerAlive(next);
                players--;
            }
            yield return GetCardGeneralStore(next, 0);
            boardController.DisableGeneralStore();
        }

        private IEnumerator GetCardGeneralStore(int player, int choice)
        {
            yield return PimPamPumEvent(playerControllers[player] + " has chosen the card: " + generalStoreChoices[choice]);
            boardController.RemoveGeneralStoreCard(choice);
            playerControllers[player].AddCard(generalStoreChoices[choice]);
            generalStoreChoices.RemoveAt(choice);
        }

        public IEnumerator StartDuel(int player, int target)
        {
            int next = player;
            int pimPamPumsTarget = 0;

            decision = Decision.Pending;

            yield return playerControllers[target].AvoidCard(player, target);

            if (decision != Decision.Avoid)
            {

                yield return PimPamPumEvent("Starts the duel between: " + playerControllers[player] + " and " + playerControllers[target]);

                do
                {
                    next = next == player ? target : player;
                    playerControllers[next].EnablePimPamPumsDuelResponse();
                    float time = 0;
                    decision = Decision.Pending;
                    while (decision == Decision.Pending && time < decisionTime)
                    {
                        time += Time.deltaTime;
                        yield return null;
                    }
                    if (decision == Decision.Pending)
                    {
                        decision = Decision.TakeHit;
                    }
                    else if (decision == Decision.Avoid)
                    {
                        yield return PimPamPumEvent(playerControllers[next] + " keeps dueling.");
                        if (next == target)
                        {
                            pimPamPumsTarget++;
                        }
                    }
                } while (decision != Decision.TakeHit);

                playerControllers[player].CheckNoCards();
                playerControllers[target].FinishDuelTarget(pimPamPumsTarget);

                yield return HitPlayer(player, next);
            }
            else
            {
                yield return DiscardUsedCard(target);
            }
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

        public IEnumerator Dying(int target)
        {
            PlayerController pc = playerControllers[target];
            float time = 0;
            decision = Decision.Pending;
            while (decision != Decision.Die && time < decisionTime && pc.IsDying)
            {
                time += Time.deltaTime;
                yield return null;
            }
            pc.EnableDieButton(false);
        }

        public IEnumerator PimPamPum(int player, int target, int misses = 1)
        {
            yield return PimPamPumTo(player, target, misses);
            if (decision == Decision.TakeHit)
            {
                yield return HitPlayer(player, target);
            }
        }

        public IEnumerator HitPlayer(int player, int target)
        {
            yield return playerControllers[target].GetHitBy(player);
        }

        private IEnumerator PimPamPumTo(int player, int target, int misses = 1)
        {
            PlayerController targetPc = playerControllers[target];
            float time = 0;
            int dodges = 0, barrelsUsed = 0, barrels = targetPc.Barrels;
            bool dodge;
            Card barrelDrawn;
            decision = Decision.Pending;
            while (dodges < misses && decision != Decision.TakeHit)
            {
                targetPc.EnableMissedsResponse();
                if (barrelsUsed < barrels)
                {
                    targetPc.EnableBarrelButton(true);
                }
                while (time < decisionTime && decision == Decision.Pending)
                {
                    time += Time.deltaTime;
                    yield return null;
                }
                if (decisionTime - time < 0.01f)
                {
                    decision = Decision.TakeHit;
                }
                else if (decision == Decision.Avoid)
                {
                    decision = Decision.Pending;
                    dodges++;
                    yield return CardResponse(target);
                }
                else if (decision == Decision.Barrel)
                {
                    decision = Decision.Pending;
                    barrelsUsed++;
                    barrelDrawn = BarrelDodge(out dodge);
                    dodges += dodge ? 1 : 0;
                    yield return BarrelEffect(target, barrelDrawn, dodge);
                }
            }
        }

        private Card BarrelDodge(out bool dodged)
        {
            Card c = DrawCard();
            dodged = Barrel.CheckCondition(c);
            return c;
        }

        private IEnumerator BarrelEffect(int target, Card c, bool dodge)
        {
            yield return DrawEffect(target, c);
            yield return PimPamPumEvent(playerControllers[target] + (dodge ? " barrel succesfully used as a Missed!" : " the barrel didn't help.") + " Card: " + c);
        }

        public IEnumerator PimPamPumEvent(string pimPamPumEvent)
        {
            yield return new WaitForSeconds(pimPamPumEventTime);
            Debug.Log(pimPamPumEvent);
        }

        public IEnumerator PimPamPumEventPlayedCard(int player, int target, Card card, Drop drop, int cardIndex)
        {
            PlayerController pc = playerControllers[player];
            PlayerController pcTarget = playerControllers[target];
            yield return PimPamPumEvent(pc + " Card: " + card + " Target: " + pcTarget + " Drop: " + drop + " CardIndex: " + cardIndex);
        }

        public IEnumerator PimPamPumEventHitBy(int player, int target)
        {
            PlayerController pc = playerControllers[player];
            PlayerController pcTarget = playerControllers[target];
            yield return PimPamPumEvent(pcTarget + " has been hit by " + pc);
        }

        public IEnumerator Indians(int player, Card c)
        {
            PlayerController pc;
            float time = 0;
            for (int i = player == MaxPlayers - 1 ? 0 : player + 1; i != player; i = i == MaxPlayers - 1 ? 0 : i + 1)
            {
                pc = playerControllers[i];
                if (!pc.IsDead && !pc.Immune(c))
                {
                    decision = Decision.Pending;
                    pc.EnablePimPamPumsResponse();
                    while (time < decisionTime && decision == Decision.Pending)
                    {
                        time += Time.deltaTime;
                        yield return null;
                    }
                    if (decision == Decision.Avoid)
                    {
                        yield return CardResponse(i);
                    }
                    else
                    {
                        yield return pc.Hit(player);
                    }
                }
            }
            yield return MultipleTargetResponsesFinished(player);
        }

        private IEnumerator MultipleTargetResponsesFinished(int player)
        {
            for (int i = player == MaxPlayers - 1 ? 0 : player + 1; i != player; i = i == MaxPlayers - 1 ? 0 : i + 1)
            {
                yield return playerControllers[i].Dying(player);
            }
            for (int i = player == MaxPlayers - 1 ? 0 : player + 1; i != player; i = i == MaxPlayers - 1 ? 0 : i + 1)
            {
                yield return playerControllers[i].Die(player);
            }
        }

        public IEnumerator AvoidCard(int player, int target)
        {
            playerControllers[target].EnableMissedsResponse();
            float time = 0;
            float total = decisionTime / 2;
            while (time < total && decision == Decision.Pending)
            {
                time += Time.deltaTime;
                yield return null;
            }
        }

        public IEnumerator DiscardUsedCard(int target)
        {
            yield return PimPamPumEvent(playerControllers[target] + " has avoided the card effect with: " + cardUsed);
            DiscardCard(cardUsed);
        }

        private IEnumerator CardResponse(int target)
        {
            PlayerController pc = playerControllers[target];
            int playerNum = pc.PlayerNumber;
            Card used = cardUsed;
            yield return PimPamPumEvent(pc + " has avoided the hit with: " + cardUsed);
            pc.Response();
            DiscardCard(used);
        }

        public IEnumerator Gatling(int player, Card c)
        {
            PlayerController pc;
            for (int i = player == MaxPlayers - 1 ? 0 : player + 1; i != player; i = i == MaxPlayers - 1 ? 0 : i + 1)
            {
                pc = playerControllers[i];
                if (!pc.IsDead && !pc.Immune(c))
                {
                    yield return PimPamPumTo(player, i);
                    if (decision == Decision.TakeHit)
                    {
                        yield return pc.Hit(player);
                    }
                }
            }
            yield return MultipleTargetResponsesFinished(player);
        }

        public IEnumerator LemonadeJimBeerUsed(int player)
        {
            float time = 0;
            decision = Decision.Pending;
            while (time < decisionTime && decision == Decision.Pending)
            {
                time += Time.deltaTime;
                yield return null;
            }
            if(decision == Decision.Heal)
            {
                yield return PimPamPumEvent(playerControllers[player] + " has used his special ability and healed 1 HP.");
            }
        }

        public void Saloon()
        {
            for (int i = 0; i < MaxPlayers; i++)
            {
                playerControllers[i].HealFromSaloon();
            }
        }

        public void SetMatch(PlayerController[] players)
        {
            playerControllers = players;
            MaxPlayers = players.Length;

            foreach (PlayerController pc in playerControllers)
                foreach (PlayerController pc2 in playerControllers)
                    pc.Setup(pc2.connectionToClient, pc2.PlayerNumber);

            StartGame();
        }

        public IEnumerator DiscardCardEndTurn(Card c, int player)
        {
            PickedCard = false;
            for (int i = player == MaxPlayers - 1 ? 0 : player + 1; i != player; i = i == MaxPlayers - 1 ? 0 : i + 1)
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

        public void AddPlayerController(int i, PlayerController pc)
        {
            playerControllers[i] = pc;
        }

        public IEnumerator DiscardCopiesOf<T>(int player, Property p) where T : Property
        {
            for (int i = player == MaxPlayers - 1 ? 0 : player + 1; i != player; i = i == MaxPlayers - 1 ? 0 : i + 1)
            {
                yield return playerControllers[i].DiscardCopiesOf<T>(p);
            }
        }

        public IEnumerator ChooseCardToPutOnDeckTop(int player)
        {
            PlayerController pc = playerControllers[player];
            NetworkConnection conn = pc.connectionToClient;
            generalStoreChoices = boardController.DrawKitCarlsonCards(conn);
            generalStoreChoice = -1;

            float time = 0;
            while(time < decisionTime && generalStoreChoice < 0)
            {
                time += Time.deltaTime;
                yield return null;
            }

            boardController.RemoveCardsAndDisableGeneralStore(conn);

            if(generalStoreChoice < 0)
            {
                generalStoreChoice = Random.Range(0, generalStoreChoices.Count);
            }

            Card choice = generalStoreChoices[generalStoreChoice];
            generalStoreChoices.RemoveAt(generalStoreChoice);

            pc.AddCard(generalStoreChoices[0]);
            pc.AddCard(generalStoreChoices[1]);
            boardController.AddCardToDeck(choice);
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

        public override void OnStartServer()
        {
            base.OnStartServer();
            Initialize();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            Initialize();
        }

        private void Initialize()
        {
            Instance = this;
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
            int nextPlayer = CurrentPlayer < MaxPlayers - 1 ? CurrentPlayer + 1 : 0;
            StartTurn(nextPlayer);
        }

        private void StartTurn(int player)
        {
            if (CurrentPlayer != PimPamPumConstants.NoOne) playerControllers[CurrentPlayer].SetTurn(false);
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
            if (playerNum < MaxPlayers - 1) return;
            foreach (PlayerController pc in playerControllers)
                pc.SetPlayerName();
        }

    }
}
