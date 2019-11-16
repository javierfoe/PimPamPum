using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

namespace PimPamPum
{
    public class GameController : NetworkBehaviour
    {
        public static GameController Instance
        {
            get; private set;
        }

        public static float TurnTime => Instance.turnTime;
        public static float ReactionTime => Instance.reactionTime;
        public static bool HasDiscardStackCards => Instance.boardController.DiscardStackSize > 0;
        public static bool FinalDuel => Instance.PlayersAlive < 3;

        private static WaitFor turnTimerCorutine;

        [Header("Player Views")]
        [SerializeField] private Transform players = null;
        [Header("Times")]
        [SerializeField] private float reactionTime = 0;
        [SerializeField] private float pimPamPumEventTime = 0, turnTime = 0;
        [Header("Card prefabs")]
        [SerializeField] private CardView cardPrefab = null;
        [SerializeField] private PropertyView propertyPrefab = null;
        [Header("Controllers")]
        [SerializeField] private SelectCardController selectCardController = null;
        [SerializeField] private BoardController boardController = null;

        private IPlayerView[] playerViews;
        private PlayerController[] playerControllers;
        public GameObject CardPrefab => cardPrefab.gameObject;
        public GameObject PropertyPrefab => propertyPrefab.gameObject;

        public static bool CheckCondition<T>(Card card) where T : ICondition, new()
        {
            ICondition checkCondition = new T();
            return checkCondition.CheckCondition(card);
        }

        public int MaxPlayers
        {
            get; set;
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

        public int NextPlayerAlive(int player)
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

        public IEnumerator UsedCard<T>(PlayerController pc) where T : Card
        {
            int player = pc.PlayerNumber;
            PlayerController aux;
            for (int i = player, j = 0; j < MaxPlayers; i = i == MaxPlayers - 1 ? 0 : i + 1, j++)
            {
                aux = playerControllers[i];
                if (!aux.IsDead)
                    yield return playerControllers[i].UsedCard<T>(player);
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

        public Card GetDiscardTopCard()
        {
            return boardController.GetDiscardTopCard();
        }

        public void SetPhaseOneDiscardClickable(int player)
        {
            boardController.EnableClickableDiscard(playerControllers[player].connectionToClient, true);
        }

        public void SetClickablePlayers(int player, List<int> targets)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            foreach (int i in targets)
            {
                playerControllers[i].EnableClick(conn, true);
            }
        }

        public void SetPhaseOnePlayerClickable(int player, List<int> targets)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            SetClickablePlayers(player, targets);
            boardController.EnableClickableDeck(conn, true);
        }

        public bool SetPhaseOnePlayerHandsClickable(int player)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            PlayerController pc;
            bool cards = false;
            for (int i = 0; i < playerControllers.Length; i++)
            {
                pc = playerControllers[i];
                if (i != player && pc.HasCards)
                {
                    cards = true;
                    pc.EnableClickHand(conn, true);
                }
            }
            if (cards)
                boardController.EnableClickableDeck(conn, true);
            return cards;
        }

        public bool SetPhaseOnePlayerPropertiesClickable(int player)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            PlayerController pc;
            bool cards = false;
            for (int i = 0; i < playerControllers.Length; i++)
            {
                pc = playerControllers[i];
                if (i != player && (pc.HasProperties || !pc.HasColt45))
                {
                    cards = true;
                    pc.EnableClickProperties(conn, true);
                }
            }
            if (cards)
                boardController.EnableClickableDeck(conn, true);
            return cards;
        }

        public void DisablePhaseOneClickable(int player)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            boardController.EnableClickableDiscard(conn, false);
            foreach (PlayerController pc in playerControllers)
            {
                if (pc.PlayerNumber != player)
                {
                    pc.EnableClick(conn, false);
                    pc.EnableClickProperties(conn, false);
                }
            }
        }

        public void SetSelectableCards(List<Card> cards, NetworkConnection conn = null)
        {
            selectCardController.SetCards(cards, conn);
        }

        public void DisableSelectableCards()
        {
            selectCardController.Disable();
        }

        public void EnableGeneralStoreCards(NetworkConnection conn, bool value)
        {
            selectCardController.EnableCards(conn, value);
        }

        public void RemoveSelectableCardsAndDisable(NetworkConnection conn)
        {
            selectCardController.RemoveCardsAndDisable(conn);
        }

        public IEnumerator DiscardEffect(int player, Card c)
        {
            yield return DiscardDrawEffect(player, c, true);
        }

        public IEnumerator DrawEffect(int player, Card c)
        {
            yield return DiscardDrawEffect(player, c, false);
        }

        private IEnumerator DiscardDrawEffect(int player, Card c, bool discardDrawEffect)
        {
            PlayerController pc;
            if (DiscardDrawEffectTriggers(player, c, discardDrawEffect, out pc))
            {
                string eventText = discardDrawEffect ?
                    " adds the discarded card to his hand: " :
                    " adds the draw! effect card to his hand: ";
                yield return PimPamPumEvent(pc + eventText + c);
                pc.AddCard(c);
            }
            else
            {
                DiscardCard(c);
            }
        }

        private bool DiscardDrawEffectTriggers(int player, Card c, bool discardDrawEffect, out PlayerController pc)
        {
            pc = null;
            bool res = false;
            PlayerController aux;
            for (int i = player, j = 0; j < MaxPlayers; i = i == MaxPlayers - 1 ? 0 : i + 1, j++)
            {
                aux = playerControllers[i];
                res |= discardDrawEffect ? aux.EndTurnDiscardPickup(player) : aux.DrawEffectPickup(player);
                pc = pc == null && res ? aux : pc;
            }
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

        public void EquipPropertyTo(int target, Property p)
        {
            p.EquipProperty(playerControllers[target]);
        }

        public IEnumerator YoulGrinnerSkill(int player)
        {
            yield return new YoulGrinnerSkillCoroutine(player, playerControllers);
        }

        public IEnumerator CatBalou(int player, int target, Drop drop, int cardIndex)
        {
            PlayerController pc = playerControllers[player];
            PlayerController targetPc = playerControllers[target];
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

        public IEnumerator Panic(int player, int target, Drop drop, int cardIndex)
        {
            PlayerController pc = playerControllers[player];
            PlayerController targetPc = playerControllers[target];
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

        public IEnumerator GeneralStore(int player)
        {
            int players = PlayersAlive;
            List<Card> cardChoices = boardController.DrawCards(players);
            SetSelectableCards(cardChoices);
            yield return new GeneralStoreCoroutine(playerControllers, player, cardChoices);
        }

        public IEnumerator GetCardGeneralStore(int player, int choice, Card card)
        {
            yield return PimPamPumEvent(playerControllers[player] + " has chosen the card: " + card);
            selectCardController.RemoveCard(choice);
            playerControllers[player].AddCard(card);
        }

        public IEnumerator StartDuel(int player, int target)
        {
            yield return new DuelCoroutine(playerControllers[player], playerControllers[target]);
        }

        public void TradeTwoForOne(int player, int cardIndex, int target)
        {
            PlayerController pc = playerControllers[player];
            PlayerController targetPc = playerControllers[target];
            Card c = pc.UnequipHandCard(cardIndex);
            for (int i = 0; i < 2 && targetPc.Hand.Count > 0; i++)
            {
                Card targetCard = targetPc.GetCardFromHand();
                pc.AddCard(targetCard);
            }
            targetPc.AddCard(c);
        }

        public void StealProperty(int player, int target, int index)
        {
            PlayerController pc = playerControllers[player];
            PlayerController targetPc = playerControllers[target];
            Card c = targetPc.UnequipProperty(index);
            pc.AddCard(c);
            targetPc.StolenBy(player);
        }

        public void StealWeapon(int player, int target)
        {
            PlayerController pc = playerControllers[player];
            PlayerController targetPc = playerControllers[target];
            Card c = targetPc.UnequipWeapon();
            pc.AddCard(c);
            targetPc.StolenBy(player);
        }

        public void StealCard(int player, int target)
        {
            StealIfHandNotEmpty(player, target);
            playerControllers[target].StolenBy(player);
        }

        public void StealIfHandNotEmpty(int player, int target)
        {
            PlayerController pc = playerControllers[player];
            PlayerController targetPc = playerControllers[target];
            if (targetPc.HasCards)
            {
                Card c = targetPc.StealCardFromHand();
                pc.AddCard(c);
            }
        }

        public IEnumerator PimPamPum(int player, int target, int misses = 1)
        {
            PlayerController targetPc = playerControllers[target];
            PimPamPumCoroutine pimPamPumCoroutine = new PimPamPumCoroutine(targetPc, misses);
            yield return pimPamPumCoroutine;
            if (pimPamPumCoroutine.TakeHit)
            {
                yield return HitPlayer(player, target);
            }
        }

        public IEnumerator HitPlayer(int player, PlayerController target)
        {
            yield return target.GetHitBy(player);
        }

        public IEnumerator HitPlayer(int player, int target)
        {
            yield return HitPlayer(player, playerControllers[target]);
        }

        public IEnumerator BarrelEffect(int target, Card c, bool dodge)
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
            yield return new MultiTargetingCoroutine<IndianCoroutine>(playerControllers, player, c);
        }

        public IEnumerator CardResponse(int player, Card card)
        {
            PlayerController pc = playerControllers[player];
            yield return PimPamPumEvent(pc + " has avoided the hit with: " + card);
            pc.Response();
            DiscardCard(card);
        }

        public IEnumerator Gatling(int player, Card c)
        {
            yield return new MultiTargetingCoroutine<PimPamPumCoroutine>(playerControllers, player, c);
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
            boardController.ConstructorBoard();
            playerControllers = players;
            MaxPlayers = players.Length;

            foreach (PlayerController pc in playerControllers)
            {
                foreach (PlayerController pc2 in playerControllers)
                {
                    pc.Setup(pc2.connectionToClient, pc2.PlayerNumber);
                }
                pc.Setup();
            }

            StartGame();
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

        public IEnumerator DiscardCopiesOf<T>(int player, Property p) where T : Property, new()
        {
            for (int i = player == MaxPlayers - 1 ? 0 : player + 1; i != player; i = i == MaxPlayers - 1 ? 0 : i + 1)
            {
                yield return playerControllers[i].DiscardCopiesOf<T>(p);
            }
        }

        public IEnumerator ChooseCardToPutOnDeckTop(int player)
        {
            PlayerController pc = playerControllers[player];

            WaitForCardSelection chooseCardTimer = new WaitForCardSelection(pc, 3);
            yield return chooseCardTimer;

            pc.AddCards(chooseCardTimer.NotChosenCards);
            boardController.AddCardToDeck(chooseCardTimer.ChosenCard);
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
            int sheriff = -1;
            for (int i = 0; sheriff < 0 && i < playerControllers.Length; i++)
            {
                sheriff = playerControllers[i].Role == Role.Sheriff ? i : -1;
            }
            StartTurn(sheriff);
        }

        public void EndTurn(int player)
        {
            if (CurrentPlayer != player) return;
            WaitForController.StopTurnCorutine();
            int nextPlayer = NextPlayerAlive(player);
            StartTurn(nextPlayer);
        }

        private void StartTurn(int player)
        {
            if (CurrentPlayer != PimPamPumConstants.NoOne) playerControllers[CurrentPlayer].SetTurn(false);
            if (turnTimerCorutine != null) turnTimerCorutine.StopCorutine();
            CurrentPlayer = player;
            PlayerController current = playerControllers[player];
            turnTimerCorutine = WaitFor.StartTurnCorutine(current);
            StartCoroutine(current.TurnTimer(turnTimerCorutine));
            current.StartTurn();
        }

        public List<int> PlayersInWeaponRange(int player, Card c = null)
        {
            return PlayersInRange(player, playerControllers[player].WeaponRange, c);
        }

        public List<int> PlayersInRange(int player, int range, Card c = null)
        {
            List<int> res = new List<int>();

            TraversePlayers(res, player, range, true);
            TraversePlayers(res, player, range, false);

            return res;
        }

        private void TraversePlayers(List<int> players, int player, int range, bool forward, Card c = null)
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
                if (!players.Contains(next) && !dead && next != player && pc.RangeModifier + auxRange < range + 1 && (c == null || c != null && !pc.Immune(c))) players.Add(next);
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

        public void TargetSelfProperty<T>(int player) where T : Property, new()
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

        public void TargetOthersWithHand(int player, Card c)
        {
            NetworkConnection conn = playerControllers[player].connectionToClient;
            foreach (PlayerController pc in playerControllers)
                if (pc.PlayerNumber != player && !pc.IsDead && !pc.Immune(c) && pc.Hand.Count > 0)
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
    }
}
