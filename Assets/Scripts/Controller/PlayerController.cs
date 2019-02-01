﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Bang
{
    public abstract class PlayerController : NetworkBehaviour
    {

        public static PlayerController LocalPlayer { get; private set; }

        protected static GameController GameController { get; set; }

        private static Colt45 colt45 = new Colt45();

        [SyncVar] private int playerNum;

        [SerializeField] private int characterHP = 4;

        private Coroutine hit, jailCheck;
        private int hp;
        private Weapon weapon;
        private List<Card> properties;
        private IPlayerView playerView;
        private bool endTurn, jail, dynamite;
        private string playerName;
        private Card draggedCard;

        protected int bangsUsed;
        protected List<Card> hand;

        public State State
        {
            get; private set;
        }

        public int DraggedCardIndex
        {
            get; set;
        }

        public IPlayerView PlayerView
        {
            protected get
            {
                return playerView;
            }
            set
            {
                playerView = value;
                playerView.SetPlayerIndex(PlayerNumber);
            }
        }

        private string PlayerName
        {
            get { return playerName; }
            set
            {
                playerName = value;
                CmdSetPlayerName(value);
            }
        }

        protected int HP
        {
            get { return hp; }
            set
            {
                hp = value;
                RpcUpdateHP(hp);
            }
        }

        public bool Stealable
        {
            get
            {
                return HasCards || HasProperties || !HasColt45;
            }
        }

        public bool HasCards
        {
            get
            {
                return hand.Count > 0;
            }
        }

        public bool HasProperties
        {
            get
            {
                return properties.Count > 0;
            }
        }

        public bool HasColt45
        {
            get
            {
                return Weapon == colt45;
            }
        }

        public bool IsDead
        {
            get; private set;
        }

        public bool IsDying
        {
            get { return HP < 1; }
        }

        protected int MaxHP
        {
            get; set;
        }

        protected int BeerHeal
        {
            get; set;
        }

        protected int Phase1CardsDrawn
        {
            get; set;
        }

        public int PlayerNumber
        {
            get { return playerNum; }
            set { playerNum = value; }
        }

        public int MissesToDodge
        {
            get; protected set;
        }

        public int Scope
        {
            get; protected set;
        }

        public int RangeModifier
        {
            get; protected set;
        }

        public int Barrels
        {
            get; protected set;
        }

        protected bool ActivePlayer
        {
            get { return GameController.CurrentPlayer == PlayerNumber; }
        }

        public Role Role
        {
            get; private set;
        }

        public Weapon Weapon
        {
            get { return weapon; }
            set
            {
                weapon = value;
                RpcEquipWeapon(weapon.Struct);
            }
        }

        public override void OnStartServer()
        {
            State = State.OutOfTurn;
            BeerHeal = 1;
            Phase1CardsDrawn = 2;
            MissesToDodge = 1;
            MaxHP = characterHP;
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
            GameController.SetPlayerViews();
            PlayerView = GameController.GetPlayerView(0);
            PlayerView.SetLocalPlayer();
            PlayerName = NetworkManagerButton.PlayerName;
            PlayerName = ToString();
            PlayerView.SetPlayerName(PlayerName);
        }

        public bool BelongsToTeam(Team team)
        {
            return
                (team == Team.Law && (Role == Role.Sheriff || Role == Role.Deputy)) ||
                (team == Team.Outlaw && Role == Role.Outlaw);
        }

        public virtual void SetRole(Role role)
        {
            Role = role;
            if (role == Role.Sheriff)
            {
                MaxHP++;
                RpcSheriff();
            }
            else
            {
                TargetSetRole(connectionToClient, role);
            }
            HP = MaxHP;
            RpcSetCharacter(Character());
            Weapon = colt45;
            DrawInitialCards();
        }

        public IEnumerator DrawFromCard(int amount)
        {
            Draw(amount);
            yield return null;
        }

        public void Draw(int amount = 1)
        {
            List<Card> cards = GameController.DrawCards(amount);
            foreach (Card card in cards)
            {
                AddCard(card);
            }
        }

        private IEnumerator Phase1()
        {
            yield return DrawPhase1();
            Phase2();
        }

        protected virtual IEnumerator DrawPhase1()
        {
            Draw(Phase1CardsDrawn);
            yield return null;
        }

        protected virtual void DrawInitialCards()
        {
            Draw(hp);
        }

        public void AddCard(Card c)
        {
            hand.Add(c);
            TargetAddCard(connectionToClient, hand.Count - 1, c.Struct);
            RpcAddCard();
        }

        public void EnableProperties(bool value)
        {
            if (value)
            {
                foreach (Property p in properties)
                    p.AddPropertyEffect(this);
            }
            else
            {
                foreach (Property p in properties)
                    p.RemovePropertyEffect(this);
            }
        }

        public Card UnequipDraggedCard()
        {
            Card c = hand[DraggedCardIndex];
            RemoveCardFromHand(DraggedCardIndex);
            return c;
        }

        private void RemoveCardFromHand(int index)
        {
            hand.RemoveAt(index);
            TargetRemoveCard(connectionToClient, index);
            RpcRemoveCard();
        }

        public void EquipPropertyTo(int target, Property p)
        {
            if (target == PlayerNumber)
            {
                p.EquipProperty(this);
                return;
            }
            GameController.EquipPropertyTo(target, p);
        }

        public IEnumerator DiscardCopiesOf<T>(Property p) where T : Property
        {
            if (HasProperty<T>())
            {
                int index;
                Property foundProperty = FindProperty<T>(out index);
                if (foundProperty != p)
                {
                    yield return BangEvent(this + " has discarded: " + foundProperty);
                    DiscardProperty(index);
                }
            }
            else if (Weapon is T && Weapon != p)
            {
                yield return BangEvent(this + " has discarded: " + Weapon);
                DiscardWeapon();
            }
        }

        public virtual IEnumerator Equip<T>(Property p) where T : Property
        {
            yield return null;
        }

        public void EquipProperty(Property c)
        {
            properties.Add(c);
            RpcEquipProperty(properties.Count - 1, c.Struct);
        }

        public void EquipBarrel()
        {
            Barrels++;
        }

        public void UnequipBarrel()
        {
            Barrels--;
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
            endTurn = false;
            SetTurn(true);
            StartCoroutine(OnStartTurn());
        }

        public void SetTurn(bool value)
        {
            RpcSetTurn(value);
        }

        protected void Phase2()
        {
            State = State.Play;
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

        public void FinishDuelTarget(int bangsUsed = 1)
        {
            for (int i = 0; i < bangsUsed; i++) CardUsedOutOfTurn();
            CheckNoCards();
        }

        public void Response()
        {
            FinishDuelTarget();
        }

        public void CheckNoCards()
        {
            if (!HasCards)
            {
                NoCardTrigger();
            }
        }

        protected virtual void NoCardTrigger() { }

        protected virtual void CardUsedOutOfTurn() { }

        public IEnumerator DrawEffect(Card c)
        {
            if (!IsDead)
            {
                yield return DrawEffectTrigger(c);
            }
        }

        protected virtual IEnumerator DrawEffectTrigger(Card c)
        {
            yield return null;
        }

        private IEnumerator DynamiteCheck()
        {
            yield return GameController.DrawEffect(PlayerNumber);
            int index;
            Dynamite d = FindProperty<Dynamite>(out index);
            UnequipProperty(index);
            if (Dynamite.CheckCondition(GameController.DrawnCard))
            {
                yield return BangEvent(this + ": Dynamite exploded. 3 damage inflicted");
                GameController.DiscardCard(d);
                yield return GetHitBy(BangConstants.NoOne, 3);
            }
            else
            {
                yield return BangEvent(this + ": Avoids the dynamite and passes it to the next player");
                GameController.PassDynamite(PlayerNumber, d);
            }
        }

        public IEnumerator JailCheck()
        {
            yield return GameController.DrawEffect(PlayerNumber);
            int index;
            Jail j = FindProperty<Jail>(out index);
            endTurn = !Jail.CheckCondition(GameController.DrawnCard);
            UnequipProperty(index);
            yield return BangEvent(this + (endTurn ? " stays in prison." : " has escaped the prison. "));
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

        public virtual void ForceEndTurn()
        {
            State = State.OutOfTurn;
            DisableCards();
            GameController.EndTurn();
        }

        protected void ConvertHandTo<T>() where T : Card, new()
        {
            int length = hand.Count;
            for (int i = 0; i < length; i++)
            {
                hand[i] = hand[i].ConvertTo<T>();
            }
        }

        protected void ConvertHandCardTo<O, D>() where O : Card, new() where D : Card, new()
        {
            Card c;
            int length = hand.Count;
            for (int i = 0; i < length; i++)
            {
                c = hand[i];
                if (c is O)
                {
                    hand[i] = c.ConvertTo<D>();
                }
            }
        }

        protected void OriginalHand()
        {
            Card c, original;
            int length = hand.Count;
            for (int i = 0; i < length; i++)
            {
                c = hand[i];
                original = c.Original;
                if (original != null)
                {
                    hand[i] = original;
                }
            }
        }

        public virtual void DisableCards()
        {
            EnableTakeHitButton(false);
            EnableEndTurnButton(false);
            int length = hand.Count;
            for (int i = 0; i < length; i++)
            {
                TargetEnableCard(connectionToClient, i, false);
            }
        }

        private void DiscardEndTurn()
        {
            State = State.Discard;
            int length = hand.Count;
            for (int i = 0; i < length; i++)
            {
                TargetEnableCard(connectionToClient, i, true);
            }
        }

        protected virtual void EnableCardsPlay()
        {
            State = State.Play;
            EnableCards();
        }

        private void EnableCardsDying()
        {
            EnableDieButton(true);
            State = State.Dying;
            EnableCards();
        }

        public void EnableMissedsResponse()
        {
            EnableTakeHitButton(true);
            State = State.Response;
            EnableCards(CardType.Missed);
        }

        public void EnableBangsResponse()
        {
            EnableTakeHitButton(true);
            State = State.Response;
            EnableCards(CardType.Bang);
        }

        public void EnableBangsDuelResponse()
        {
            EnableTakeHitButton(true);
            State = State.Duel;
            EnableCards();
        }

        protected virtual void EnableCards(CardType card = 0)
        {
            switch (State)
            {
                case State.Play:
                    EnablePhase2Cards();
                    break;
                case State.Dying:
                    EnableDyingReaction();
                    break;
                case State.Duel:
                    EnableDuelReaction();
                    break;
                case State.Response:
                    switch (card)
                    {
                        case CardType.Bang:
                            EnableIndiansReaction();
                            break;
                        case CardType.Missed:
                            EnableBangReaction();
                            break;
                    }
                    break;
            }
        }

        protected virtual void EnablePhase2Cards()
        {
            bool bangs = Weapon.Bang(this);
            int length = hand.Count;
            Card c;
            bool isMissed;
            bool isBang;
            for (int i = 0; i < length; i++)
            {
                c = hand[i];
                isMissed = c is Missed;
                isBang = c is Bang;
                TargetEnableCard(connectionToClient, i, !isBang && !isMissed || !isMissed && bangs);
            }
        }

        protected virtual void EnableDyingReaction()
        {
            EnableReactionCards<Beer>();
        }

        private void EnableDuelReaction()
        {
            EnableBangCardsForReaction();
        }

        protected virtual void EnableIndiansReaction()
        {
            EnableBangCardsForReaction();
        }

        protected virtual void EnableBangCardsForReaction()
        {
            EnableReactionCards<Bang>();
        }

        protected virtual void EnableBangReaction()
        {
            EnableReactionCards<Missed>();
        }

        protected void EnableReactionCards<T>() where T : Card, new()
        {
            int length = hand.Count;
            for (int i = 0; i < length; i++)
            {
                TargetEnableCard(connectionToClient, i, hand[i] is T);
            }
        }

        private IEnumerator PlayCard(int player, Drop drop, int cardIndex)
        {
            DisableCards();
            yield return hand[DraggedCardIndex].PlayCard(this, player, drop, cardIndex);
            if (!ActivePlayer)
            {
                CardUsedOutOfTurn();
            }
        }

        private void DuelResponse(int index)
        {
            MakeDecision(Decision.Avoid, index);
            DiscardCardFromHand(index);
        }

        public IEnumerator DiscardCardEndTurn(int index)
        {
            DisableCards();
            Card c = UnequipHandCard(index);
            yield return GameController.DiscardCardEndTurn(c, PlayerNumber);
            EndTurn();
        }

        public IEnumerator Saloon()
        {
            GameController.Saloon();
            yield return null;
        }

        public IEnumerator ShotBang(int target)
        {
            bangsUsed++;
            yield return ShotBangTrigger(target);
        }

        protected virtual IEnumerator ShotBangTrigger(int target)
        {
            yield return GameController.Bang(PlayerNumber, target, MissesToDodge);
        }

        public IEnumerator Indians()
        {
            yield return GameController.Indians(PlayerNumber, draggedCard);
        }

        public IEnumerator Gatling()
        {
            yield return GameController.Gatling(PlayerNumber, draggedCard);
        }

        public IEnumerator GetHitBy(int player, int amount = 1)
        {
            yield return Hit(player, amount);
            yield return Dying(player, amount);
            yield return Die(player);
        }

        public IEnumerator EndTurnDiscard(Card c)
        {
            if (!IsDead)
            {
                yield return EndTurnDiscardTrigger(c);
            }
        }

        protected virtual IEnumerator EndTurnDiscardTrigger(Card c)
        {
            yield return null;
        }

        public virtual bool Bang()
        {
            bool res = true;
            if (bangsUsed > 0) res = false;
            return res;
        }

        public void EquipWeapon(Weapon weapon)
        {
            if (!HasColt45)
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
            if (value && !Stealable) return;
            SetStealable(conn, value, HasCards, !HasColt45);
        }

        public void BeginCardDrag(Card c)
        {
            draggedCard = c;
        }

        public void BangBeginCardDrag()
        {
            GameController.TargetPlayersRange(PlayerNumber, weapon.Range + Scope, draggedCard);
        }

        public void JailBeginCardDrag()
        {
            GameController.TargetPrison(PlayerNumber, draggedCard);
        }

        public void CatBalouBeginCardDrag()
        {
            GameController.TargetAllCards(PlayerNumber, draggedCard);
        }

        public void PanicBeginCardDrag()
        {
            GameController.TargetAllRangeCards(PlayerNumber, 1 + Scope, draggedCard);
        }

        public void TargetOthers()
        {
            GameController.TargetOthers(PlayerNumber, draggedCard);
        }

        public void SelfTargetCard()
        {
            GameController.TargetSelf(PlayerNumber);
        }

        public void SelfTargetPropertyCard<T>() where T : Property
        {
            GameController.TargetSelfProperty<T>(PlayerNumber);
        }

        public void StopTargeting(NetworkConnection conn)
        {
            TargetSetTargetable(conn, false);
            SetStealable(conn, false);
        }

        public virtual bool Immune(Card c)
        {
            return false;
        }

        protected virtual IEnumerator OnStartTurn()
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
                yield return JailCheck();
                if (endTurn)
                {
                    ForceEndTurn();
                }
                else
                {
                    yield return Phase1();
                }
            }
            else
            {
                yield return Phase1();
            }
        }

        public IEnumerator Hit(int attacker, int amount = 1)
        {
            if (attacker != BangConstants.NoOne && attacker != PlayerNumber)
            {
                yield return GameController.BangEventHitBy(attacker, PlayerNumber);
            }
            else
            {
                yield return BangEvent(this + " loses " + amount + " hit points.");
            }
            HP -= amount;
            EnableTakeHitButton(false);
        }

        public IEnumerator Dying(int attacker, int amount = 1)
        {
            if (!IsDead)
            {
                if (IsDying)
                {
                    EnableCardsDying();
                    yield return GameController.Dying(PlayerNumber);
                    DisableCards();
                }
                if (!IsDead)
                {
                    for (int i = 0; i < amount; i++) HitTrigger(attacker);
                }
            }
        }

        protected virtual void HitTrigger(int attacker) { }

        public IEnumerator Die(int killer)
        {
            if (!IsDead && IsDying)
            {
                yield return DieTrigger(killer);
            }
        }

        protected virtual IEnumerator DieTrigger(int killer)
        {
            yield return BangEvent(this + " has died.");
            IsDead = true;
            if (Role != Role.Sheriff) RpcSetRole(Role);
            List<Card> deadCards = new List<Card>();
            for (int i = hand.Count - 1; i > -1; i--)
            {
                deadCards.Add(UnequipHandCard(i));
            }
            for (int i = properties.Count - 1; i > -1; i--)
            {
                deadCards.Add(UnequipProperty(i));
            }
            Card weapon = UnequipWeapon();
            if (weapon != null) deadCards.Add(weapon);

            GameController.CheckDeath(deadCards);
            GameController.CheckMurder(killer, PlayerNumber);
        }

        public void DiscardAll()
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

        public virtual bool CheckDeath(List<Card> list)
        {
            return false;
        }

        public virtual IEnumerator HealFromBeer()
        {
            yield return GameController.UsedBeer(PlayerNumber);
            if (!GameController.FinalDuel) Heal(BeerHeal);
        }

        public IEnumerator UsedBeer()
        {
            if (!IsDead)
            {
                yield return UsedBeerTrigger();
            }
        }

        protected virtual IEnumerator UsedBeerTrigger()
        {
            yield return null;
        }

        public virtual void HealFromSaloon()
        {
            if (IsDead) return;
            Heal();
        }

        public void Heal(int amount = 1)
        {
            if (HP + amount < MaxHP) HP += amount;
            else HP = MaxHP;
        }

        public void DiscardCardUsed()
        {
            DiscardCardFromHand(DraggedCardIndex);
        }

        public void FinishCardUsed()
        {
            if (IsDead)
            {
                ForceEndTurn();
                return;
            }
            CheckNoCards();
            if (ActivePlayer)
                Phase2();
        }

        public void DiscardCardFromHand(int index)
        {
            Card card = UnequipHandCard(index);
            GameController.DiscardCard(card);
        }

        public IEnumerator GeneralStore()
        {
            yield return GameController.GeneralStore(PlayerNumber);
        }

        public IEnumerator Duel(int player)
        {
            yield return GameController.StartDuel(PlayerNumber, player);
            State = State.Play;
        }

        public IEnumerator CatBalou(int target, Drop drop, int cardIndex)
        {
            yield return GameController.CatBalou(PlayerNumber, target, drop, cardIndex);
        }

        public IEnumerator Panic(int target, Drop drop, int cardIndex)
        {
            yield return GameController.Panic(PlayerNumber, target, drop, cardIndex);
        }

        public virtual IEnumerator AvoidCard(int player, int target)
        {
            yield return null;
        }

        public virtual IEnumerator StolenBy(int thief)
        {
            CheckNoCards();
            yield return null;
        }

        public void DiscardWeapon()
        {
            if (HasColt45) return;
            Weapon weapon = UnequipWeapon();
            GameController.DiscardCard(weapon);
        }

        public Weapon UnequipWeapon()
        {
            if (HasColt45) return null;
            Weapon weapon = Weapon;
            Weapon.RemovePropertyEffect(this);
            Weapon = colt45;
            return weapon;
        }

        public Card StealCardFromHand(int index = -1)
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
            card.RemovePropertyEffect(this);
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

        private void MakeDecision(Decision decision, int index = -1)
        {
            DisableCards();
            Card card = index > -1 ? hand[index] : null;
            GameController.MakeDecision(PlayerNumber, card, decision);
        }

        public void Win()
        {
            TargetWin(connectionToClient);
        }

        public void Lose()
        {
            TargetLose(connectionToClient);
        }

        private void SetStealable(NetworkConnection conn, bool value, bool hand = true, bool weapon = true)
        {
            TargetSetStealable(conn, value, hand, weapon);
        }

        protected virtual int CardLimit()
        {
            return HP;
        }

        private void EndTurn()
        {
            if (hand.Count <= CardLimit())
            {
                WillinglyEndTurn();
            }
            else
            {
                DiscardEndTurn();
            }
        }

        protected virtual void WillinglyEndTurn()
        {
            ForceEndTurn();
        }

        public void SetTargetable(NetworkConnection conn, bool value)
        {
            TargetSetTargetable(conn, value);
        }

        public void Setup(NetworkConnection conn, int playerIndex)
        {
            TargetSetup(conn, playerIndex);
        }

        public IEnumerator BangEvent(string bangEvent)
        {
            yield return GameController.BangEvent(bangEvent);
        }

        public IEnumerator BangEventPlayedCard(Card card, int target, Drop drop, int cardIndex)
        {
            yield return GameController.BangEventPlayedCard(PlayerNumber, target, card, drop, cardIndex);
        }

        public override string ToString()
        {
            return PlayerName;
        }

        protected abstract string Character();

        public void SetPlayerName()
        {
            RpcSetPlayerName(PlayerName);
        }

        public void EnableBarrelButton(bool value)
        {
            TargetEnableBarrelButton(connectionToClient, value);
        }

        private void BangResponseButton()
        {
            PlayerView.EnableTakeHitButton(false);
            PlayerView.EnableBarrelButton(false);
        }

        protected void AvoidButton()
        {
            TargetAvoidButton(connectionToClient);
        }

        protected void TakeHitButton()
        {
            TargetTakeHitButton(connectionToClient);
        }

        [Client]
        public void ChooseGeneralStoreCard(int index)
        {
            CmdChooseGeneralStoreCard(index);
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
            BangResponseButton();
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
        }

        [Client]
        public void UseBarrel()
        {
            BangResponseButton();
            CmdMakeDecision(Decision.Barrel);
        }

        [Client]
        public void EndTurnButton()
        {
            PlayerView.EnableEndTurnButton(false);
            CmdEndTurn();
        }

        [Command]
        private void CmdChooseGeneralStoreCard(int choice)
        {
            GameController.ChooseGeneralStoreCard(choice);
        }

        [Command]
        private void CmdMakeDecision(Decision decision)
        {
            MakeDecision(decision);
        }

        [Command]
        private void CmdBeginCardDrag(int index)
        {
            DraggedCardIndex = index;
            GameController.HighlightTrash(PlayerNumber, true);
            if (State == State.Play)
            {
                hand[DraggedCardIndex].BeginCardDrag(this);
            }
        }

        [Command]
        private void CmdEndTurn()
        {
            EndTurn();
        }

        [Command]
        private void CmdUseCard(int index, int player, Drop drop, int cardIndex)
        {
            switch (State)
            {
                case State.Play:
                    if (player > -1)
                    {
                        StartCoroutine(PlayCard(player, drop, cardIndex));
                    }
                    break;
                case State.Discard:
                    if (drop == Drop.Trash)
                    {
                        StartCoroutine(DiscardCardEndTurn(index));
                    }
                    break;
                case State.Dying:
                    if (drop == Drop.Trash)
                    {
                        StartCoroutine(PlayCard(PlayerNumber, drop, cardIndex));
                    }
                    break;
                case State.Duel:
                    if (drop == Drop.Trash)
                    {
                        DuelResponse(index);
                    }
                    break;
                case State.Response:
                    if (drop == Drop.Trash)
                    {
                        MakeDecision(Decision.Avoid, index);
                        UnequipHandCard(index);
                    }
                    break;
            }
            GameController.StopTargeting(PlayerNumber);
        }

        [Command]
        private void CmdSetPlayerName(string name)
        {
            playerName = name;
            GameController.SetPlayerNames(PlayerNumber);
        }

        [ClientRpc]
        private void RpcEquipWeapon(CardStruct cs)
        {
            PlayerView.EquipWeapon(cs);
        }

        [ClientRpc]
        private void RpcEquipProperty(int index, CardStruct cs)
        {
            PlayerView.EquipProperty(index, cs);
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

        [ClientRpc]
        private void RpcSetRole(Role role)
        {
            PlayerView.SetRole(role);
        }

        [ClientRpc]
        private void RpcSetCharacter(string character)
        {
            PlayerView.SetCharacter(character);
        }

        [ClientRpc]
        private void RpcSetTurn(bool value)
        {
            PlayerView.SetTurn(value);
        }

        [ClientRpc]
        public void RpcSetPlayerName(string name)
        {
            playerName = name;
            PlayerView.SetPlayerName(name);
        }

        [TargetRpc]
        private void TargetSetTargetable(NetworkConnection conn, bool value)
        {
            PlayerView.SetDroppable(value);
        }

        [TargetRpc]
        private void TargetSetStealable(NetworkConnection conn, bool value, bool hand, bool weapon)
        {
            PlayerView.SetStealable(value, hand, weapon);
        }

        [TargetRpc]
        protected void TargetEnableCard(NetworkConnection conn, int card, bool value)
        {
            PlayerView.EnableCard(card, value);
        }

        [TargetRpc]
        private void TargetAddCard(NetworkConnection conn, int index, CardStruct cs)
        {
            PlayerView.AddCard(index, cs);
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
        private void TargetAvoidButton(NetworkConnection conn)
        {
            PlayerView.SetTextTakeHitButton("Take card effect");
        }

        [TargetRpc]
        private void TargetTakeHitButton(NetworkConnection conn)
        {
            PlayerView.SetTextTakeHitButton("Take hit");
        }

        [TargetRpc]
        private void TargetSetup(NetworkConnection conn, int playerNumber)
        {
            if (!isLocalPlayer)
            {
                PlayerView = GameController.GetPlayerView(playerNumber, this.PlayerNumber);
            }
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

        [TargetRpc]
        private void TargetEnableBarrelButton(NetworkConnection conn, bool value)
        {
            PlayerView.EnableBarrelButton(value);
        }

        [TargetRpc]
        private void TargetWin(NetworkConnection conn)
        {
            PlayerView.Win();
        }

        [TargetRpc]
        private void TargetLose(NetworkConnection conn)
        {
            PlayerView.Lose();
        }

    }
}