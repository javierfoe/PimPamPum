using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Bang
{
    public class PlayerController : NetworkBehaviour
    {

        private enum State
        {
            Play,
            Response,
            Duel,
            Discard,
            Dying
        }

        private enum HitState
        {
            Draw,
            Play
        }

        [SyncVar] private int playerNum;

        public static PlayerController LocalPlayer { get; private set; }
        private static GameController GameController { get; set; }
        private static Colt45 colt45 = new Colt45();
        private const int NoOne = -1;

        private Coroutine hit, jailCheck;
        private int draggedCard, bangsUsed, hp, maxHp;
        private State state;
        private HitState hitState;
        private Weapon weapon;
        private List<Card> hand, properties;
        private IPlayerView playerView;
        private bool endTurn, jail, dynamite;

        public IPlayerView PlayerView
        {
            private get
            {
                return playerView;
            }
            set
            {
                playerView = value;
                playerView.SetPlayerIndex(playerNum);
            }
        }

        private int HP
        {
            get { return hp; }
            set
            {
                hp = value;
                RpcUpdateHP(hp);
            }
        }

        public bool IsDead
        {
            get { return HP < 1; }
        }

        private int MaxHP
        {
            get { return maxHp; }
            set
            {
                maxHp = value;
                HP = value;
            }
        }

        public int PlayerNumber
        {
            get { return playerNum; }
            set { playerNum = value; }
        }

        public int Scope
        {
            get; private set;
        }

        public int RangeModifier
        {
            get; private set;
        }

        public Role Role
        {
            private set; get;
        }

        public Weapon Weapon
        {
            get { return weapon; }
            set
            {
                weapon = value;
                RpcEquipWeapon(weapon.ToString(), weapon.Suit, weapon.Rank, weapon.Color);
            }
        }

        public override void OnStartServer()
        {
            hand = new List<Card>();
            properties = new List<Card>();
        }

        public override void OnStartClient()
        {
            if (!GameController) GameController = FindObjectOfType<GameController>();
        }

        public override void OnStartLocalPlayer()
        {
            LocalPlayer = this;
        }

        public virtual void SetRole(Role role)
        {
            Role = role;
            if (role == Role.Sheriff)
            {
                MaxHP = 5;
                RpcSheriff();
            }
            else
            {
                MaxHP = 4;
                TargetSetRole(connectionToClient, role);
            }
            Weapon = colt45;
            DrawInitialCards();
        }

        public void DrawFromCard(int amount)
        {
            DiscardCardUsed();
            Draw(amount);
        }

        public void Draw(int amount)
        {
            List<Card> cards = GameController.DrawCards(amount);
            foreach (Card card in cards)
            {
                AddCard(card);
            }
        }

        protected virtual void DrawInitialCards()
        {
            Draw(hp);
        }

        private void AddCard(Card c)
        {
            hand.Add(c);
            TargetAddCard(connectionToClient, hand.Count - 1, c.ToString(), c.Suit, c.Rank, c.Color);
            RpcAddCard();
        }

        public Card UnequipDraggedCard()
        {
            Card c = hand[draggedCard];
            RemoveCardFromHand(draggedCard);
            return c;
        }

        private void RemoveCardFromHand(int index)
        {
            hand.RemoveAt(index);
            TargetRemoveCard(connectionToClient, index);
            RpcRemoveCard();
        }

        public void Imprison(int target, Jail c)
        {
            PlayerController pc = GameController.GetPlayerController(target);
            c.EquipProperty(pc);
        }

        public void EquipProperty(Property c)
        {
            properties.Add(c);
            RpcEquipProperty(properties.Count - 1, c.ToString(), c.Suit, c.Rank, c.Color);
        }

        public void EquipJail()
        {
            jail = true;
        }

        public void UnequipJail()
        {
            jail = false;
        }

        public void EquipDynamite()
        {
            dynamite = true;
        }

        public void UnequipDynamite()
        {
            dynamite = false;
        }

        public void EquipScope()
        {
            Scope++;
        }

        public void UnequipScope()
        {
            Scope--;
        }

        public void EquipMustang()
        {
            RangeModifier++;
        }

        public void UnequipMustang()
        {
            RangeModifier--;
        }

        public void StartTurn()
        {
            hitState = HitState.Draw;
            endTurn = false;
            StartCoroutine(OnStartTurn());
        }

        private void Phase1()
        {
            hitState = HitState.Play;
            Draw(2);
            bangsUsed = 0;
            Phase2();
        }

        private void Phase2()
        {
            if (!IsDead)
            {
                EnableCardsPlay();
                EnableEndTurnButton(true);
            }
            else
            {
                ForceEndTurn();
            }
        }

        public void FinishDuelTarget(int bangsUsed)
        {
            for (int i = 0; i < bangsUsed; i++) CardUsedOutOfTurn();
            CheckNoCards();
        }

        public virtual void CheckNoCards() { }

        protected virtual void CardUsedOutOfTurn() { }

        public void DyingFinished()
        {
            switch (hitState)
            {
                case HitState.Play:
                    Phase2();
                    break;
            }
        }

        private IEnumerator DynamiteCheck()
        {
            Card c = GameController.DrawDiscardCard();
            int index;
            Dynamite d = FindProperty<Dynamite>(out index);
            UnequipProperty(index);
            if (d.CheckCondition(c))
            {
                GameController.DiscardCard(d);
                yield return Hit(NoOne, 3);
            }
            else
            {
                GameController.PassDynamite(playerNum, d);
            }
        }

        public void JailCheck()
        {
            Card c = GameController.DrawDiscardCard();
            int index;
            Jail j = FindProperty<Jail>(out index);
            endTurn = !j.CheckCondition(c);
            UnequipProperty(index);
            GameController.DiscardCard(j);
        }

        private T FindProperty<T>(out int index) where T : Card
        {
            bool found = false;
            T res = null;
            Card c;
            index = -1;
            for (int i = 0; i < properties.Count && !found; i++)
            {
                c = properties[i];
                found = c is T;
                index = found ? i : index;
                res = found ? (T)c : res;
            }
            return res;
        }

        public void ForceEndTurn()
        {
            GameController.EndTurn();
        }

        public void DisableCards()
        {
            EnableEndTurnButton(false);
            int length = hand.Count;
            for (int i = 0; i < length; i++)
            {
                TargetEnableCard(connectionToClient, i, false);
            }
        }

        private void DiscardEndTurn()
        {
            state = State.Discard;
            int length = hand.Count;
            for (int i = 0; i < length; i++)
            {
                TargetEnableCard(connectionToClient, i, true);
            }
        }

        protected virtual void EnableCardsPlay()
        {
            state = State.Play;
            EnableNotCards<Missed>();
        }

        private void EnableCardsDying()
        {
            EnableDieButton(true);
            state = State.Dying;
            EnableCards<Beer>();
        }

        public void EnableCardsResponse<T>() where T : Card
        {
            EnableTakeHitButton(true);
            state = State.Response;
            EnableCards<T>();
        }

        public void EnableCardsDuelResponse()
        {
            EnableTakeHitButton(true);
            state = State.Duel;
            EnableCards<Bang>();
        }

        protected virtual void EnableNotCards<T>()
        {
            bool bangs = Weapon.Bang(this);
            int length = hand.Count;
            Card c;
            bool isT;
            bool isBang;
            for (int i = 0; i < length; i++)
            {
                c = hand[i];
                isT = c is T;
                isBang = c is Bang;
                TargetEnableCard(connectionToClient, i, !isBang && !isT || !isT && bangs);
            }
        }

        protected virtual void EnableCards<T>()
        {
            int length = hand.Count;
            for (int i = 0; i < length; i++)
            {
                TargetEnableCard(connectionToClient, i, hand[i] is T);
            }
        }

        public void PlayCard(int player, Drop drop, int cardIndex)
        {
            hand[draggedCard].PlayCard(this, player, drop, cardIndex);
        }

        public void DiscardCardEndTurn(int index)
        {
            DiscardCardFromHandEndTurn(index);
        }

        public void Saloon()
        {
            DiscardCardUsed();
            GameController.Saloon();
        }

        public void ShotBang(int target)
        {
            DiscardCardUsed();
            DisableCards();
            bangsUsed++;
            StartCoroutine(GameController.WaitForBangResponse(playerNum, target, 1));
        }

        public void Indians()
        {
            DiscardCardUsed();
            DisableCards();
            StartCoroutine(GameController.WaitForIndiansResponse(playerNum));
        }

        public void Gatling()
        {
            DiscardCardUsed();
            DisableCards();
            StartCoroutine(GameController.WaitForGatlingResponse(playerNum));
        }

        public void ResponsesFinished()
        {
            Phase2();
        }

        public virtual bool Bang()
        {
            bool res = true;
            if (bangsUsed > 0) res = false;
            return res;
        }

        public void EquipWeapon(Weapon weapon)
        {
            if (Weapon != colt45)
            {
                GameController.DiscardCard(Weapon);
            }
            Weapon = weapon;
        }

        public bool HasProperty<T>() where T : Property
        {
            return Has<T>(properties);
        }

        public bool HasHand<T>() where T : Card
        {
            return Has<T>(hand);
        }

        private bool Has<T>(List<Card> list) where T : Card
        {
            bool res = false;
            int length = list.Count;
            for (int i = 0; i < length && !res; i++)
            {
                res = list[i] is T;
            }
            return res;
        }

        public void SetStealable(NetworkConnection conn, bool value)
        {
            TargetSetStealable(conn, value, weapon != colt45);
        }

        public void BangBeginCardDrag()
        {
            GameController.TargetPlayersRange(playerNum, weapon.Range + Scope);
        }

        public void JailBeginCardDrag()
        {
            GameController.TargetPrison(playerNum);
        }

        public void CatBalouBeginCardDrag()
        {
            GameController.TargetAllCards(playerNum);
        }

        public void PanicBeginCardDrag()
        {
            GameController.TargetAllRangeCards(playerNum, 1 + Scope);
        }

        public void TargetOthers()
        {
            GameController.TargetOthers(playerNum);
        }

        public void SelfTargetCard()
        {
            GameController.TargetSelf(playerNum);
        }

        public void SelfTargetPropertyCard<T>() where T : Property
        {
            GameController.TargetSelfProperty<T>(playerNum);
        }

        public void StopTargeting(NetworkConnection conn)
        {
            TargetSetTargetable(conn, false);
            TargetSetStealable(conn, false, true);
        }

        private IEnumerator OnStartTurn()
        {
            bangsUsed = 0;
            if (dynamite)
            {
                yield return DynamiteCheck();
            }
            if (IsDead)
            {
                ForceEndTurn();
            }
            else if (jail)
            {
                JailCheck();
                if (endTurn)
                {
                    ForceEndTurn();
                }
                else
                {
                    Phase1();
                }
            }
            else
            {
                Phase1();
            }
        }

        public IEnumerator Hit(int attacker, int amount = 1)
        {
            EnableTakeHitButton(false);
            HP -= amount;
            for (int i = 0; i < amount; i++) HitTrigger();
            if (IsDead)
            {
                EnableCardsDying();
                yield return GameController.Dying(playerNum, attacker);
                DisableCards();
            }
        }

        public virtual void HitTrigger() { }

        public virtual void Die(int killer)
        {
            for (int i = hand.Count - 1; i > -1; i--)
            {
                DiscardCardFromHand(i);
            }
            for (int i = properties.Count - 1; i > -1; i--)
            {
                DiscardProperty(i);
            }
            DiscardWeapon();
        }

        public virtual void HealFromBeer()
        {
            DiscardCardUsed();
            if (!GameController.FinalDuel) Heal();
        }

        public void Heal(int amount = 1)
        {
            if (HP + amount < MaxHP) HP += amount;
            else HP = MaxHP;
        }

        private void DiscardCardUsed()
        {
            DiscardCardFromHand(draggedCard);
            CheckNoCards();
        }

        public void FinishCardUsed()
        {
            Phase2();
        }

        public void DiscardCardResponse(int index)
        {
            DiscardCardFromHand(index);
            CardUsedOutOfTurn();
            CheckNoCards();
        }

        public void DiscardCardFromHand(int index)
        {
            Card card = UnequipHandCard(index);
            GameController.DiscardCard(card);
        }

        public void Duel(int player)
        {
            DiscardCardUsed();
            DisableCards();
            StartCoroutine(GameController.StartDuel(playerNum, player));
        }

        public void CatBalou(int player, Drop drop, int cardIndex)
        {
            PlayerController pc = GameController.GetPlayerController(player);
            Card c = null;
            switch (drop)
            {
                case Drop.Hand:
                    if (playerNum == player && cardIndex < draggedCard) draggedCard--;
                    c = pc.StealHandFromHand(cardIndex);
                    break;
                case Drop.Properties:
                    c = pc.UnequipProperty(cardIndex);
                    break;
                case Drop.Weapon:
                    c = pc.UnequipWeapon();
                    break;
            }
            pc.StolenBy(this);
            DiscardCardUsed();
            GameController.DiscardCard(c);
        }

        public void Panic(int player, Drop drop, int cardIndex)
        {
            PlayerController pc = GameController.GetPlayerController(player);
            Card c = null;
            switch (drop)
            {
                case Drop.Hand:
                    if (player == playerNum)
                    {
                        c = null;
                    }
                    else
                    {
                        c = pc.StealHandFromHand(cardIndex);
                    }
                    break;
                case Drop.Properties:
                    c = pc.UnequipProperty(cardIndex);
                    break;
                case Drop.Weapon:
                    c = pc.UnequipWeapon();
                    break;
            }
            pc.StolenBy(this);
            DiscardCardUsed();
            if (c != null) AddCard(c);
        }

        protected virtual void StolenBy(PlayerController thief) { }

        public void DiscardWeapon()
        {
            if (Weapon == colt45) return;
            Weapon weapon = UnequipWeapon();
            GameController.DiscardCard(weapon);
        }

        public Weapon UnequipWeapon()
        {
            Weapon weapon = Weapon;
            Weapon.UnequipProperty(this);
            Weapon = colt45;
            return weapon;
        }

        public Card StealHandFromHand(int index)
        {
            if (index < 0)
            {
                index = Random.Range(0, hand.Count - 1);
            }
            return UnequipHandCard(index);
        }

        public Card UnequipHandCard(int index)
        {
            Card card = hand[index];
            hand.RemoveAt(index);
            TargetRemoveCard(connectionToClient, index);
            RpcRemoveCard();
            return card;
        }

        public Card UnequipProperty(int index)
        {
            Property card = (Property)properties[index];
            properties.RemoveAt(index);
            card.UnequipProperty(this);
            RpcRemoveProperty(index);
            return card;
        }

        public void DiscardProperty(int index)
        {
            Card card = UnequipProperty(index);
            DiscardProperty(card);
        }

        public void DiscardProperty(Card card)
        {
            GameController.DiscardCard(card);
        }

        public void DiscardCardFromHandEndTurn(int index)
        {
            DiscardCardFromHand(index);
            if (hand.Count < hp + 1)
            {
                GameController.EndTurn();
                DisableCards();
            }
        }

        public void EnableTakeHitButton(bool value)
        {
            TargetEnableTakeHitButton(connectionToClient, value);
        }

        public void EnableEndTurnButton(bool value)
        {
            TargetEnableEndTurnButton(connectionToClient, value);
        }

        public void EnableDieButton(bool value)
        {
            TargetEnableDieButton(connectionToClient, value);
        }

        private void MakeDecision(Decision decision)
        {
            GameController.MakeDecision(playerNum, decision);
        }

        [Client]
        public void WillinglyDie()
        {
            PlayerView.EnableDieButton(false);
            CmdMakeDecision(Decision.Die);
        }

        [Client]
        public void TakeHit()
        {
            PlayerView.EnableTakeHitButton(false);
            CmdMakeDecision(Decision.TakeHit);
        }

        [Client]
        public void BeginCardDrag(int index)
        {
            CmdBeginCardDrag(index);
        }

        [Client]
        public void UseCard(int index, int player, Drop drop, int cardIndex)
        {
            CmdUseCard(index, player, drop, cardIndex);
            CmdStopTargeting();
        }

        [Client]
        public void EndTurn()
        {
            PlayerView.EnableEndTurnButton(false);
            CmdEndTurn();
        }

        [Command]
        public void CmdMakeDecision(Decision decision)
        {
            MakeDecision(decision);
        }

        [Command]
        public void CmdBeginCardDrag(int index)
        {
            draggedCard = index;
            if (state == State.Play) hand[draggedCard].BeginCardDrag(this);
        }

        [Command]
        public void CmdEndTurn()
        {
            DisableCards();
            if (hand.Count < hp + 1)
            {
                GameController.EndTurn();
            }
            else
            {
                DiscardEndTurn();
            }
        }

        [Command]
        public void CmdStopTargeting()
        {
            GameController.StopTargeting(playerNum);
        }

        [Command]
        private void CmdUseCard(int index, int player, Drop drop, int cardIndex)
        {
            switch (state)
            {
                case State.Play:
                    if (player > -1)
                        PlayCard(player, drop, cardIndex);
                    break;
                case State.Discard:
                    if (drop == Drop.Trash)
                        DiscardCardEndTurn(index);
                    break;
                case State.Dying:
                    if (drop == Drop.Trash)
                        PlayCard(playerNum, drop, cardIndex);
                    break;
                case State.Duel:
                    if (drop == Drop.Trash)
                    {
                        MakeDecision(Decision.Avoid);
                        DiscardCardFromHand(index);
                    }
                    break;
                case State.Response:
                    if (drop == Drop.Trash)
                    {
                        MakeDecision(Decision.Avoid);
                        DiscardCardResponse(index);
                    }
                    break;
            }
            draggedCard = -1;
        }

        [ClientRpc]
        private void RpcEquipWeapon(string name, Suit suit, Rank rank, Color color)
        {
            PlayerView.EquipWeapon(name, suit, rank, color);
        }

        [ClientRpc]
        private void RpcEquipProperty(int index, string name, Suit suit, Rank rank, Color color)
        {
            PlayerView.EquipProperty(index, name, suit, rank, color);
        }

        [ClientRpc]
        private void RpcUpdateHP(int hp)
        {
            PlayerView.UpdateHP(hp);
        }

        [ClientRpc]
        private void RpcAddCard()
        {
            if (isLocalPlayer) return;
            PlayerView.AddCard();
        }

        [ClientRpc]
        private void RpcRemoveCard()
        {
            if (isLocalPlayer) return;
            PlayerView.RemoveCard();
        }

        [ClientRpc]
        private void RpcRemoveProperty(int index)
        {
            PlayerView.RemoveProperty(index);
        }

        [ClientRpc]
        private void RpcSheriff()
        {
            PlayerView.SetSheriff();
        }

        [TargetRpc]
        public void TargetSetTargetable(NetworkConnection conn, bool value)
        {
            PlayerView.SetDroppable(value);
        }

        [TargetRpc]
        public void TargetSetStealable(NetworkConnection conn, bool value, bool weapon)
        {
            PlayerView.SetStealable(value, weapon);
        }

        [TargetRpc]
        private void TargetEnableCard(NetworkConnection conn, int card, bool value)
        {
            PlayerView.EnableCard(card, value);
        }

        [TargetRpc]
        private void TargetAddCard(NetworkConnection conn, int index, string name, Suit suit, Rank rank, Color color)
        {
            PlayerView.AddCard(index, name, suit, rank, color);
        }

        [TargetRpc]
        private void TargetRemoveCard(NetworkConnection conn, int index)
        {
            PlayerView.RemoveCard(index);
        }

        [TargetRpc]
        private void TargetSetRole(NetworkConnection conn, Role role)
        {
            PlayerView.SetRole(role);
        }

        [TargetRpc]
        public void TargetSetup(NetworkConnection conn, int playerNumber)
        {
            IPlayerView ipv = null;
            if (isLocalPlayer)
            {
                ipv = GameController.GetPlayerView(0);
                ipv.SetClientButtons();
            }
            else
            {
                ipv = GameController.GetPlayerView(playerNumber, this.PlayerNumber);
            }
            PlayerView = ipv;
        }

        [TargetRpc]
        private void TargetEnableTakeHitButton(NetworkConnection conn, bool value)
        {
            PlayerView.EnableTakeHitButton(value);
        }

        [TargetRpc]
        private void TargetEnableEndTurnButton(NetworkConnection conn, bool value)
        {
            PlayerView.EnableEndTurnButton(value);
        }

        [TargetRpc]
        private void TargetEnableDieButton(NetworkConnection conn, bool value)
        {
            PlayerView.EnableDieButton(value);
        }

    }
}