using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PimPamPum
{
    public class GameController : MonoBehaviour
    {
        private static GameController instance;

        public static float TurnTime => instance.turnTime;
        public static float ReactionTime => instance.reactionTime;
        public static float EventTime => instance.pimPamPumEventTime;
        public static bool HasDiscardStackCards => boardController.DiscardStackSize > 0;
        public static bool FinalDuel => PlayersAlive < 3;
        public static GameObject CardPrefab => instance.cardPrefab.gameObject;
        public static GameObject PropertyPrefab => instance.propertyPrefab.gameObject;
        private static SelectCardController selectCardController => instance._selectCardController;
        private static BoardController boardController => instance._boardController;

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
        [SerializeField] private SelectCardController _selectCardController = null;
        [SerializeField] private BoardController _boardController = null;

        private static IPlayerView[] playerViews;
        private static PlayerController[] playerControllers;

        public static int MaxPlayers
        {
            get; set;
        }

        public static int CurrentPlayer
        {
            get; private set;
        }

        public static int PlayersAlive
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

        public static int PlayerStandingAlone
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

        public static bool SheriffFoesAlive
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

        public static int NextPlayerAlive(int player)
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

        private static int PreviousPlayerAlive(int player)
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

        public static void EnableOthersProperties(int player, bool value)
        {
            for (int i = player == MaxPlayers - 1 ? 0 : player + 1; i != player; i = i == MaxPlayers - 1 ? 0 : i + 1)
            {
                playerControllers[i].EnableProperties(value);
            }
        }

        public static void CheckDeath(List<Card> list)
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

        public static IEnumerator UsedCard<T>(PlayerController pc) where T : Card
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

        public static void CheckMurder(int murderer, int killed)
        {
            Role killedRole = playerControllers[killed].Role;
            if (killedRole == Role.Sheriff)
            {
                int alone = PlayerStandingAlone;
                PlayerController alonePc = playerControllers[alone];
                if (PlayersAlive == 1 && alonePc.Role == Role.Renegade)
                {
                    Lose();
                }
                else
                {
                    Lose();
                }
            }
            else if (!SheriffFoesAlive)
            {
                Win();
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

        private static void Win()
        {
            //TODO
        }

        private static void Lose()
        {
            //TODO
        }

        public static Card GetDiscardTopCard()
        {
            return boardController.GetDiscardTopCard();
        }

        public static void SetPhaseOneDiscardClickable(int player)
        {
            boardController.EnableClickableDiscard(true);
        }

        public static void SetClickablePlayers(List<int> targets)
        {
            foreach (int i in targets)
            {
                playerControllers[i].EnableClick(true);
            }
        }

        public static void SetPhaseOnePlayerClickable(List<int> targets)
        {
            SetClickablePlayers(targets);
            boardController.EnableClickableDeck(true);
        }

        public static bool SetPhaseOnePlayerHandsClickable(int player)
        {
            PlayerController pc;
            bool cards = false;
            for (int i = 0; i < playerControllers.Length; i++)
            {
                pc = playerControllers[i];
                if (i != player && pc.HasCards)
                {
                    cards = true;
                    pc.EnableClickHand(true);
                }
            }
            if (cards)
                boardController.EnableClickableDeck(true);
            return cards;
        }

        public static bool SetPhaseOnePlayerPropertiesClickable(int player)
        {
            PlayerController pc;
            bool cards = false;
            for (int i = 0; i < playerControllers.Length; i++)
            {
                pc = playerControllers[i];
                if (i != player && (pc.HasProperties || !pc.HasColt45))
                {
                    cards = true;
                    pc.EnableClickProperties(true);
                }
            }
            if (cards)
                boardController.EnableClickableDeck(true);
            return cards;
        }

        public static void DisablePhaseOneClickable(int player)
        {
            boardController.EnableClickableDiscard(false);
            foreach (PlayerController pc in playerControllers)
            {
                if (pc.PlayerNumber != player)
                {
                    pc.EnableClick(false);
                    pc.EnableClickProperties(false);
                }
            }
        }

        public static void SetSelectableCards(List<Card> cards)
        {
            selectCardController.SetCards(cards);
        }

        public static void DisableSelectableCards()
        {
            selectCardController.Disable();
        }

        public static void EnableGeneralStoreCards(bool value)
        {
            selectCardController.EnableCards(value);
        }

        public static void RemoveSelectableCardsAndDisable()
        {
            selectCardController.Disable();
        }

        public static IEnumerator DiscardEffect(int player, Card c)
        {
            yield return DiscardDrawEffect(player, c, true);
        }

        public static IEnumerator DrawEffect(int player, Card c)
        {
            yield return DiscardDrawEffect(player, c, false);
        }

        private static IEnumerator DiscardDrawEffect(int player, Card c, bool discardDrawEffect)
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

        private static bool DiscardDrawEffectTriggers(int player, Card c, bool discardDrawEffect, out PlayerController pc)
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

        public static Card DrawCard()
        {
            return boardController.DrawCard();
        }

        public static List<Card> DrawCards(int cards)
        {
            return boardController.DrawCards(cards);
        }

        public static void DiscardCard(Card card)
        {
            boardController.DiscardCard(card);
        }

        public static void EquipPropertyTo(int target, Property p)
        {
            p.EquipProperty(playerControllers[target]);
        }

        public static IEnumerator YoulGrinnerSkill(int player)
        {
            yield return new YoulGrinnerSkillCoroutine(player, playerControllers);
        }

        public static IEnumerator CatBalou(int player, int target, Drop drop, int cardIndex)
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

        public static IEnumerator Panic(int player, int target, Drop drop, int cardIndex)
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

        public static IEnumerator GeneralStore(int player)
        {
            int players = PlayersAlive;
            List<Card> cardChoices = boardController.DrawCards(players);
            SetSelectableCards(cardChoices);
            yield return new GeneralStoreCoroutine(playerControllers, player, cardChoices);
        }

        public static IEnumerator GetCardGeneralStore(int player, int choice, Card card)
        {
            yield return PimPamPumEvent(playerControllers[player] + " has chosen the card: " + card);
            selectCardController.RemoveCard(choice);
            playerControllers[player].AddCard(card);
        }

        public static IEnumerator StartDuel(int player, int target)
        {
            yield return new DuelCoroutine(playerControllers[player], playerControllers[target]);
        }

        public static void TradeTwoForOne(int player, int cardIndex, int target)
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

        public static void StealProperty(int player, int target, int index)
        {
            PlayerController pc = playerControllers[player];
            PlayerController targetPc = playerControllers[target];
            Card c = targetPc.UnequipProperty(index);
            pc.AddCard(c);
            targetPc.StolenBy(player);
        }

        public static void StealWeapon(int player, int target)
        {
            PlayerController pc = playerControllers[player];
            PlayerController targetPc = playerControllers[target];
            Card c = targetPc.UnequipWeapon();
            pc.AddCard(c);
            targetPc.StolenBy(player);
        }

        public static void StealCard(int player, int target)
        {
            StealIfHandNotEmpty(player, target);
            playerControllers[target].StolenBy(player);
        }

        public static void StealIfHandNotEmpty(int player, int target)
        {
            PlayerController pc = playerControllers[player];
            PlayerController targetPc = playerControllers[target];
            if (targetPc.HasCards)
            {
                Card c = targetPc.StealCardFromHand();
                pc.AddCard(c);
            }
        }

        public static IEnumerator PimPamPum(int player, int target, int misses = 1)
        {
            PlayerController targetPc = playerControllers[target];
            PimPamPumCoroutine pimPamPumCoroutine = new PimPamPumCoroutine(targetPc, misses);
            yield return pimPamPumCoroutine;
            if (pimPamPumCoroutine.TakeHit)
            {
                yield return HitPlayer(player, target);
            }
        }

        public static IEnumerator HitPlayer(int player, PlayerController target)
        {
            yield return target.GetHitBy(player);
        }

        public static IEnumerator HitPlayer(int player, int target)
        {
            yield return HitPlayer(player, playerControllers[target]);
        }

        public static IEnumerator BarrelEffect(int target, Card c, bool dodge)
        {
            yield return DrawEffect(target, c);
            yield return PimPamPumEvent(playerControllers[target] + (dodge ? " barrel succesfully used as a Missed!" : " the barrel didn't help.") + " Card: " + c);
        }

        public static IEnumerator PimPamPumEvent(string pimPamPumEvent)
        {
            yield return new WaitForSeconds(EventTime);
            Debug.Log(pimPamPumEvent);
        }

        public static IEnumerator PimPamPumEventPlayedCard(int player, int target, Card card, Drop drop, int cardIndex)
        {
            PlayerController pc = playerControllers[player];
            PlayerController pcTarget = playerControllers[target];
            yield return PimPamPumEvent(pc + " Card: " + card + " Target: " + pcTarget + " Drop: " + drop + " CardIndex: " + cardIndex);
        }

        public static IEnumerator PimPamPumEventHitBy(int player, int target)
        {
            PlayerController pc = playerControllers[player];
            PlayerController pcTarget = playerControllers[target];
            yield return PimPamPumEvent(pcTarget + " has been hit by " + pc);
        }

        public static IEnumerator Indians(int player, Card c)
        {
            yield return new MultiTargetingCoroutine<IndianCoroutine>(playerControllers, player, c);
        }

        public static IEnumerator CardResponse(int player, Card card)
        {
            PlayerController pc = playerControllers[player];
            yield return PimPamPumEvent(pc + " has avoided the hit with: " + card);
            pc.Response();
            DiscardCard(card);
        }

        public static IEnumerator Gatling(int player, Card c)
        {
            yield return new MultiTargetingCoroutine<PimPamPumCoroutine>(playerControllers, player, c);
        }

        public static void Saloon()
        {
            for (int i = 0; i < MaxPlayers; i++)
            {
                playerControllers[i].HealFromSaloon();
            }
        }

        public static void SetMatch(PlayerController[] players)
        {
            boardController.ConstructorBoard();
            playerControllers = players;
            MaxPlayers = players.Length;

            foreach (PlayerController pc in playerControllers)
            {
                pc.Setup();
                pc.Actions.SetPlayerStatusArray(MaxPlayers);
            }

            StartGame();
        }

        public static void SetPlayerView(int index, PlayerViewStatus playerView)
        {
            playerControllers[index].PlayerView.SetStatus(playerView);
        }

        public static void AddPlayerController(int i, PlayerController pc)
        {
            playerControllers[i] = pc;
        }

        public static IEnumerator DiscardCopiesOf<T>(int player, Property p) where T : Property, new()
        {
            for (int i = player == MaxPlayers - 1 ? 0 : player + 1; i != player; i = i == MaxPlayers - 1 ? 0 : i + 1)
            {
                yield return playerControllers[i].DiscardCopiesOf<T>(p);
            }
        }

        public static IEnumerator ChooseCardToPutOnDeckTop(int player)
        {
            PlayerController pc = playerControllers[player];

            WaitForCardSelection chooseCardTimer = new WaitForCardSelection(pc, 3);
            yield return chooseCardTimer;

            pc.AddCards(chooseCardTimer.NotChosenCards);
            boardController.AddCardToDeck(chooseCardTimer.ChosenCard);
        }

        public static void PassDynamite(int player, Dynamite d)
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

        public static void StartGame()
        {
            int sheriff = -1;
            for (int i = 0; sheriff < 0 && i < playerControllers.Length; i++)
            {
                sheriff = playerControllers[i].Role == Role.Sheriff ? i : -1;
            }
            StartTurn(sheriff);
        }

        public static void EndTurn(int player)
        {
            if (CurrentPlayer != player) return;
            WaitForController.StopTurnCorutine();
            int nextPlayer = NextPlayerAlive(player);
            StartTurn(nextPlayer);
        }

        private static void StartTurn(int player)
        {
            if (turnTimerCorutine != null) turnTimerCorutine.StopCorutine();
            CurrentPlayer = player;
            PlayerController current = playerControllers[player];
            turnTimerCorutine = WaitFor.StartTurnCorutine(current);
            instance.StartCoroutine(current.TurnTimer(turnTimerCorutine));
            current.StartTurn();
        }

        public static List<int> PlayersInWeaponRange(int player, Card c = null)
        {
            return PlayersInRange(player, playerControllers[player].WeaponRange, false, c);
        }

        public static List<int> PlayersInRange(int player, int range, bool includeItself, Card c = null)
        {
            List<int> res = new List<int>();

            TraversePlayers(res, player, range, true, includeItself);
            TraversePlayers(res, player, range, false, includeItself);

            return res;
        }

        private static void TraversePlayers(List<int> players, int player, int range, bool forward, bool includeItself, Card c = null)
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
                if (!players.Contains(next) && !dead && (includeItself || next != player) && pc.RangeModifier + auxRange < range + 1 && (c == null || c != null && !pc.Immune(c))) players.Add(next);
            } while (next != player);
        }

        public static void TargetPrison(int player, Card c)
        {
            PlayerViewStatus[] players = new PlayerViewStatus[playerViews.Length];
            PlayerController pc;
            bool droppable;
            for (int i = 0; i < playerControllers.Length; i++)
            {
                pc = playerControllers[i];
                droppable = pc.PlayerNumber != player && pc.Role != Role.Sheriff && !pc.HasProperty<Jail>() && !pc.IsDead && !pc.Immune(c);
                PlayerViewStatus status = new PlayerViewStatus()
                {
                    droppable = droppable
                };
                players[i] = status;
            }
            SetPlayersStatus(player, players);
        }

        public static void TargetAllCards(int player, Card c)
        {
            PlayerViewStatus[] players = new PlayerViewStatus[playerViews.Length];
            PlayerController pc;
            bool hand, weapon;
            for (int i = 0; i < playerControllers.Length; i++)
            {
                pc = playerControllers[i];
                hand = pc.HasCards;
                weapon = !pc.HasColt45;
                PlayerViewStatus status = new PlayerViewStatus()
                {
                    targetable = !pc.IsDead && !pc.Immune(c) && (hand || pc.HasProperties || weapon),
                    weapon = weapon,
                    hand = hand
                };
                players[i] = status;
            }
            SetPlayersStatus(player, players);
        }

        public static void TargetSelf(int player)
        {
            PlayerViewStatus[] players = new PlayerViewStatus[playerViews.Length];
            players[player].droppable = true;
            SetPlayersStatus(player, players);
        }

        public static void TargetSelfProperty<T>(int player) where T : Property, new()
        {
            PlayerController pc = playerControllers[player];
            PlayerViewStatus[] players = new PlayerViewStatus[playerViews.Length];
            players[player].droppable = !pc.HasProperty<T>();
            SetPlayersStatus(player, players);
        }

        public static void TargetOthers(int player, Card c)
        {
            PlayerViewStatus[] players = new PlayerViewStatus[playerViews.Length];
            PlayerController pc;
            for (int i = 0; i < playerControllers.Length; i++)
            {
                pc = playerControllers[i];
                PlayerViewStatus status = new PlayerViewStatus()
                {
                    droppable = pc.PlayerNumber != player && !pc.IsDead && !pc.Immune(c)
                };
                players[i] = status;
            }
            SetPlayersStatus(player, players);
        }

        public static void TargetOthersWithHand(int player, Card c)
        {
            PlayerViewStatus[] players = new PlayerViewStatus[playerViews.Length];
            PlayerController pc;
            bool droppable;
            for (int i = 0; i < playerControllers.Length; i++)
            {
                pc = playerControllers[i];
                droppable = pc.PlayerNumber != player && !pc.IsDead && !pc.Immune(c) && pc.Hand.Count > 0;
                PlayerViewStatus status = new PlayerViewStatus()
                {
                    droppable = droppable
                };
                players[i] = status;
            }
            SetPlayersStatus(player, players);
        }

        public static void TargetAllRangeCards(int player, int range, Card c)
        {
            PlayerViewStatus[] players = new PlayerViewStatus[playerViews.Length];
            List<int> playersInRange = PlayersInRange(player, range, true);
            PlayerController pc;
            bool hand, weapon;
            foreach (int i in playersInRange)
            {
                pc = playerControllers[i];
                hand = pc.HasCards;
                weapon = !pc.HasColt45;
                PlayerViewStatus status = new PlayerViewStatus()
                {
                    targetable = !pc.IsDead && !pc.Immune(c) && (hand || pc.HasProperties || weapon),
                    weapon = weapon,
                    hand = hand
                };
                players[i] = status;
            }
            SetPlayersStatus(player, players);
        }

        public static void TargetPlayersRange(int player, int range, Card c)
        {
            PlayerViewStatus[] players = new PlayerViewStatus[playerViews.Length];
            List<int> playersInRange = PlayersInRange(player, range, false);
            PlayerController pc;
            foreach (int i in playersInRange)
            {
                pc = playerControllers[i];
                PlayerViewStatus status = new PlayerViewStatus()
                {
                    droppable = !pc.IsDead && !pc.Immune(c)
                };
                players[i] = status;
            }
            SetPlayersStatus(player, players);
        }

        public static void StopTargeting(int player)
        {
            PlayerViewStatus[] players = new PlayerViewStatus[playerViews.Length];
            SetPlayersStatus(player, players);
            playerControllers[player].Actions.Thrash = false;
        }

        private static void SetPlayersStatus(int player, PlayerViewStatus[] status)
        {
            playerControllers[player].Actions.SetPlayersStatus(status);
        }

        public static void SetTargetableThrash(bool value)
        {
            boardController.SetTargetable(value);
        }

        private void Awake()
        {
            instance = this;
        }
    }
}
