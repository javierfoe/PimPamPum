using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace PimPamPum
{
    public abstract class PlayerController : NetworkBehaviour
    {
        public static PlayerController LocalPlayer { get; private set; }

        private static Colt45 colt45 = new Colt45();

        [SyncVar] private int playerNum;

        [SerializeField] private string characterName = "";
        [SerializeField] private int characterHP = 4;

        private int hp;
        private Weapon weapon;
        private List<Card> properties;
        private IPlayerView playerView;
        private bool endTurn, jail, dynamite;
        private string playerName;
        private Card draggedCard;
        protected int pimPamPumsUsed;

        public int WeaponRange => weapon.Range + Scope;
        public bool Stealable => HasCards || HasProperties || !HasColt45;
        public bool HasCards => Hand.Count > 0;
        public bool HasProperties => properties.Count > 0;
        public bool HasColt45 => Weapon == colt45;
        public bool IsDying => HP < 1;
        protected bool ActivePlayer => GameController.Instance.CurrentPlayer == PlayerNumber;

        public List<Card> Hand
        {
            get; protected set;
        }

        public State State
        {
            get; protected set;
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
                playerView.PlayerIndex = PlayerNumber;
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

        public bool IsDead
        {
            get; private set;
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

        public int DrawEffectCards
        {
            get; protected set;
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
            DrawEffectCards = 1;
            MaxHP = characterHP;
            Hand = new List<Card>();
            properties = new List<Card>();
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
            List<Card> cards = GameController.Instance.DrawCards(amount);
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
            Hand.Add(c);
            TargetAddCard(connectionToClient, Hand.Count - 1, c.Struct);
            RpcAddCard();
        }

        public void AddCards(List<Card> cards)
        {
            foreach (Card c in cards)
                AddCard(c);
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
            Card c = Hand[DraggedCardIndex];
            RemoveCardFromHand(DraggedCardIndex);
            return c;
        }

        private void RemoveCardFromHand(int index)
        {
            Hand.RemoveAt(index);
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
            GameController.Instance.EquipPropertyTo(target, p);
        }

        public IEnumerator DiscardCopiesOf<T>(Property p) where T : Property, new()
        {
            if (HasProperty<T>())
            {
                int index;
                Property foundProperty = FindProperty<T>(out index);
                if (foundProperty != p)
                {
                    yield return PimPamPumEvent(this + " has discarded: " + foundProperty);
                    DiscardProperty(index);
                }
            }
            else if (Weapon.Is<T>() && Weapon != p)
            {
                yield return PimPamPumEvent(this + " has discarded: " + Weapon);
                DiscardWeapon();
            }
        }

        public virtual IEnumerator Equip<T>(Property p) where T : Property, new()
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
            }
            else
            {
                ForceEndTurn();
            }
        }

        public void FinishResponse(int pimPamPumsUsed = 1)
        {
            for (int i = 0; i < pimPamPumsUsed; i++) CardUsedOutOfTurn();
            CheckNoCards();
        }

        public void Response()
        {
            FinishResponse();
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

        public virtual bool DrawEffectPickup(int player)
        {
            return false;
        }

        private IEnumerator DynamiteCheck()
        {
            DrawEffectCoroutine drawEffectCoroutine = new DrawEffectCoroutine(this);
            yield return drawEffectCoroutine;
            int index;
            Dynamite d = FindProperty<Dynamite>(out index);
            UnequipProperty(index);
            if (GameController.CheckCondition<Dynamite>(drawEffectCoroutine.DrawEffectCard))
            {
                yield return PimPamPumEvent(this + ": Dynamite exploded. 3 damage inflicted");
                GameController.Instance.DiscardCard(d);
                yield return GetHitBy(PimPamPumConstants.NoOne, 3);
            }
            else
            {
                yield return PimPamPumEvent(this + ": Avoids the dynamite and passes it to the next player");
                GameController.Instance.PassDynamite(PlayerNumber, d);
            }
        }

        public IEnumerator JailCheck()
        {
            DrawEffectCoroutine drawEffectCoroutine = new DrawEffectCoroutine(this);
            yield return drawEffectCoroutine;
            int index;
            Jail j = FindProperty<Jail>(out index);
            endTurn = !GameController.CheckCondition<Jail>(drawEffectCoroutine.DrawEffectCard);
            UnequipProperty(index);
            yield return PimPamPumEvent(this + (endTurn ? " stays in prison." : " has escaped the prison. "));
            GameController.Instance.DiscardCard(j);
        }

        private T FindProperty<T>(out int index) where T : Card, new()
        {
            bool found = false;
            T res = null;
            Card c;
            index = -1;
            for (int i = 0; i < properties.Count && !found; i++)
            {
                c = properties[i];
                found = c.Is<T>();
                index = found ? i : index;
                res = found ? (T)c : res;
            }
            return res;
        }

        public virtual void ForceEndTurn()
        {
            State = State.OutOfTurn;
            OriginalHand();
            DisableCards();
            GameController.Instance.EndTurn();
        }

        protected void ConvertHandTo<T>() where T : Card, new()
        {
            int length = Hand.Count;
            for (int i = 0; i < length; i++)
            {
                Hand[i] = Hand[i].ConvertTo<T>();
            }
        }

        protected void ConvertHandTo<T>(Suit suit) where T : Card, new()
        {
            int length = Hand.Count;
            for (int i = 0; i < length; i++)
            {
                Hand[i] = Hand[i].Suit == suit ? Hand[i].ConvertTo<T>() : Hand[i];
            }
        }

        protected void ConvertHandCardTo<O, D>() where O : Card, new() where D : Card, new()
        {
            Card c;
            int length = Hand.Count;
            for (int i = 0; i < length; i++)
            {
                c = Hand[i];
                if (c.Is<O>())
                {
                    Hand[i] = c.ConvertTo<D>();
                }
            }
        }

        protected void OriginalHand()
        {
            Card c, original;
            int length = Hand.Count;
            for (int i = 0; i < length; i++)
            {
                c = Hand[i];
                original = c.Original;
                if (original != null)
                {
                    Hand[i] = original;
                }
            }
        }

        public virtual void DisableCards()
        {
            EnableTakeHitButton(false);
            EnableEndTurnButton(false);
            EnableBarrelButton(false);
            int length = Hand.Count;
            for (int i = 0; i < length; i++)
            {
                TargetEnableCard(connectionToClient, i, false);
            }
            EnableSkill(false);
        }

        protected void EnableAllCards()
        {
            int length = Hand.Count;
            for (int i = 0; i < length; i++)
            {
                TargetEnableCard(connectionToClient, i, true);
            }
        }

        private void DiscardEndTurn()
        {
            State = State.Discard;
            EnableAllCards();
            EnableSkill(false);
        }

        protected virtual void EnableCardsPlay()
        {
            State = State.Play;
            EnableCards();
            EnableEndTurnButton(true);
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

        public void EnablePimPamPumsResponse()
        {
            EnableTakeHitButton(true);
            State = State.Response;
            EnableCards(CardType.PimPamPum);
        }

        public void EnablePimPamPumsDuelResponse()
        {
            EnableTakeHitButton(true);
            State = State.Duel;
            EnableCards();
        }

        public virtual void UsedSkillCard() { }

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
                        case CardType.PimPamPum:
                            EnableIndiansReaction();
                            break;
                        case CardType.Missed:
                            EnablePimPamPumReaction();
                            break;
                    }
                    break;
            }
        }

        protected virtual void EnablePhase2Cards()
        {
            bool pimPamPums = Weapon.PimPamPum(this);
            int length = Hand.Count;
            Card c;
            bool isMissed;
            bool isPimPamPum;
            for (int i = 0; i < length; i++)
            {
                c = Hand[i];
                isMissed = c.Is<Missed>();
                isPimPamPum = c.Is<PimPamPum>();
                TargetEnableCard(connectionToClient, i, !isPimPamPum && !isMissed || !isMissed && pimPamPums);
            }
        }

        protected virtual void EnableDyingReaction()
        {
            EnableCards<Beer>();
        }

        private void EnableDuelReaction()
        {
            EnablePimPamPumCardsForReaction();
        }

        protected virtual void EnableIndiansReaction()
        {
            EnablePimPamPumCardsForReaction();
        }

        protected virtual void EnablePimPamPumCardsForReaction()
        {
            EnableCards<PimPamPum>();
        }

        protected virtual void EnablePimPamPumReaction()
        {
            EnableCards<Missed>();
        }

        protected void EnableCards<T>() where T : Card
        {
            int length = Hand.Count;
            for (int i = 0; i < length; i++)
            {
                TargetEnableCard(connectionToClient, i, Hand[i].Is<T>());
            }
        }

        private IEnumerator PlayCard(int player, Drop drop, int cardIndex)
        {
            yield return Hand[DraggedCardIndex].PlayCard(this, player, drop, cardIndex);
            if (!ActivePlayer)
            {
                CardUsedOutOfTurn();
            }
        }

        private IEnumerator Response(int index)
        {
            yield return ResponseSub(index);
            UnequipHandCard(index);
        }

        private IEnumerator DuelResponse(int index)
        {
            yield return ResponseSub(index);
            DiscardCardFromHand(index);
        }

        private IEnumerator ResponseSub(int index)
        {
            yield return Hand[index].CardUsed(this);
            MakeDecisionServer(Decision.Avoid, index);
        }

        public IEnumerator DiscardCardEndTurn(int index)
        {
            DisableCards();
            Card c = UnequipHandCard(index);
            yield return GameController.Instance.DiscardEffect(PlayerNumber, c);
            EndTurn();
        }

        public IEnumerator Saloon()
        {
            GameController.Instance.Saloon();
            yield return null;
        }

        public void PimPamPumUsed()
        {
            pimPamPumsUsed++;
        }

        public void TradeTwoForOne(int player)
        {
            GameController.Instance.TradeTwoForOne(PlayerNumber, DraggedCardIndex, player);
        }

        public IEnumerator ShootPimPamPum(int target)
        {
            yield return ShootPimPamPumTrigger(target);
        }

        protected virtual IEnumerator ShootPimPamPumTrigger(int target)
        {
            yield return GameController.Instance.PimPamPum(PlayerNumber, target, MissesToDodge);
        }

        public IEnumerator Indians()
        {
            yield return GameController.Instance.Indians(PlayerNumber, draggedCard);
        }

        public IEnumerator Gatling()
        {
            yield return GameController.Instance.Gatling(PlayerNumber, draggedCard);
        }

        public IEnumerator GetHitBy(int player, int amount = 1)
        {
            yield return Hit(player, amount);
            yield return Dying(player, amount);
            yield return Die(player);
        }

        public virtual bool EndTurnDiscardPickup(int player)
        {
            return false;
        }

        public virtual bool PimPamPum()
        {
            bool res = true;
            if (pimPamPumsUsed > 0) res = false;
            return res;
        }

        public void EquipWeapon(Weapon weapon)
        {
            if (!HasColt45)
            {
                GameController.Instance.DiscardCard(Weapon);
            }
            Weapon = weapon;
        }

        public bool HasProperty<T>() where T : Property, new()
        {
            return Has<T>(properties);
        }

        public bool HasHand<T>() where T : Card, new()
        {
            return Has<T>(Hand);
        }

        private bool Has<T>(List<Card> list) where T : Card, new()
        {
            bool res = false;
            int length = list.Count;
            for (int i = 0; i < length && !res; i++)
            {
                res = list[i].Is<T>();
            }
            return res;
        }

        public void SetStealable(NetworkConnection conn, bool value)
        {
            if (value && !Stealable) return;
            SetStealable(conn, value, HasCards, !HasColt45);
        }

        public virtual void BeginCardDrag(Card c)
        {
            draggedCard = c;
        }

        public void PimPamPumBeginCardDrag()
        {
            GameController.Instance.TargetPlayersRange(PlayerNumber, weapon.Range + Scope, draggedCard);
        }

        public void JailBeginCardDrag()
        {
            GameController.Instance.TargetPrison(PlayerNumber, draggedCard);
        }

        public void CatBalouBeginCardDrag()
        {
            GameController.Instance.TargetAllCards(PlayerNumber, draggedCard);
        }

        public void PanicBeginCardDrag()
        {
            GameController.Instance.TargetAllRangeCards(PlayerNumber, 1 + Scope, draggedCard);
        }

        public void TargetOthers()
        {
            GameController.Instance.TargetOthers(PlayerNumber, draggedCard);
        }

        public void TargetOthersWithHand()
        {
            GameController.Instance.TargetOthersWithHand(PlayerNumber, draggedCard);
        }

        public void SelfTargetCard()
        {
            GameController.Instance.TargetSelf(PlayerNumber);
        }

        public void SelfTargetPropertyCard<T>() where T : Property, new()
        {
            GameController.Instance.TargetSelfProperty<T>(PlayerNumber);
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
            pimPamPumsUsed = 0;
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
            if (attacker != PimPamPumConstants.NoOne && attacker != PlayerNumber)
            {
                yield return GameController.Instance.PimPamPumEventHitBy(attacker, PlayerNumber);
            }
            else
            {
                yield return PimPamPumEvent(this + " loses " + amount + " hit points.");
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
                    yield return new WaitForDying(this);
                    EnableDieButton(false);
                    DisableCards();
                }
                if (!IsDying)
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
                IsDead = true;
                yield return DieTrigger(killer);
            }
        }

        protected virtual IEnumerator DieTrigger(int killer)
        {
            yield return PimPamPumEvent(this + " has died.");
            if (Role != Role.Sheriff) RpcSetRole(Role);
            List<Card> deadCards = new List<Card>();
            for (int i = Hand.Count - 1; i > -1; i--)
            {
                deadCards.Add(UnequipHandCard(i));
            }
            for (int i = properties.Count - 1; i > -1; i--)
            {
                deadCards.Add(UnequipProperty(i));
            }
            Card weapon = UnequipWeapon();
            if (weapon != null) deadCards.Add(weapon);

            GameController.Instance.CheckDeath(deadCards);
            GameController.Instance.CheckMurder(killer, PlayerNumber);
        }

        public void DiscardAll()
        {
            for (int i = Hand.Count - 1; i > -1; i--)
            {
                DiscardCardFromHand(i);
            }
            for (int i = properties.Count - 1; i > -1; i--)
            {
                DiscardProperty(i);
            }
            DiscardWeapon();
        }

        public bool CheckDeath(List<Card> list)
        {
            if (IsDead || IsDying) return false;
            return CheckDeathTrigger(list);
        }

        public virtual bool CheckDeathTrigger(List<Card> list)
        {
            return false;
        }

        public void HealFromBeer()
        {
            if (!GameController.FinalDuel) Heal(BeerHeal);
        }

        public virtual IEnumerator UsedCard<T>(int player) where T : Card
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
            GameController.Instance.DiscardCard(card);
        }

        public IEnumerator GeneralStore()
        {
            yield return GameController.Instance.GeneralStore(PlayerNumber);
        }

        public IEnumerator Duel(int player)
        {
            yield return GameController.Instance.StartDuel(PlayerNumber, player);
            State = State.Play;
        }

        public IEnumerator CatBalou(int target, Drop drop, int cardIndex)
        {
            yield return GameController.Instance.CatBalou(PlayerNumber, target, drop, cardIndex);
        }

        public IEnumerator Panic(int target, Drop drop, int cardIndex)
        {
            yield return GameController.Instance.Panic(PlayerNumber, target, drop, cardIndex);
        }

        public virtual IEnumerator StolenBy(int thief)
        {
            yield return null;
        }

        public void DiscardWeapon()
        {
            if (HasColt45) return;
            Weapon weapon = UnequipWeapon();
            GameController.Instance.DiscardCard(weapon);
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
            Card res = GetCardFromHand(index);
            CheckNoCards();
            return res;
        }

        public Card GetCardFromHand(int index = -1)
        {
            if (index < 0)
            {
                index = Random.Range(0, Hand.Count - 1);
            }
            Card res = UnequipHandCard(index);
            return res;
        }

        public Card UnequipHandCard(int index)
        {
            Card card = Hand[index];
            Hand.RemoveAt(index);
            TargetRemoveCard(connectionToClient, index);
            RpcRemoveCard();
            card = card.Original ?? card;
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
            GameController.Instance.DiscardCard(card);
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

        public void EnablePassButton(bool value)
        {
            TargetEnablePassButton(connectionToClient, value);
        }

        public void EnableClick(NetworkConnection conn, bool value)
        {
            TargetClickable(conn, value);
        }

        public void EnableClickHand(NetworkConnection conn, bool value)
        {
            TargetClickableHand(conn, value);
        }

        public void EnableClickProperties(NetworkConnection conn, bool value)
        {
            TargetClickableProperties(conn, value, !HasColt45);
        }

        protected void MakeDecisionServer(Decision decision, int index = -1)
        {
            DisableCards();
            Card card = index > -1 ? Hand[index] : null;
            WaitFor.CurrentWaitFor.MakeDecision(decision, card);
        }

        public void MakeDecisionClient(Decision decision)
        {
            CmdMakeDecision(decision);
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
            if (Hand.Count <= CardLimit())
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

        public IEnumerator PimPamPumEvent(string pimPamPumEvent)
        {
            yield return GameController.Instance.PimPamPumEvent(pimPamPumEvent);
        }

        public override string ToString()
        {
            return PlayerName;
        }

        private string Character()
        {
            return characterName;
        }

        protected virtual void EnableSkill(bool value)
        {
            TargetEnableSkill(connectionToClient, value);
        }

        protected void SetSkillStatus(bool value)
        {
            TargetSetSkillStatus(connectionToClient, value);
        }

        public void SetPlayerName()
        {
            RpcSetPlayerName(PlayerName);
        }

        public void EnableBarrelButton(bool value)
        {
            TargetEnableBarrelButton(connectionToClient, value);
        }

        protected void EnableConfirmButton(bool value)
        {
            TargetEnableConfirmButton(connectionToClient, value);
        }

        protected void EnableCancelButton(bool value)
        {
            TargetEnableCancelButton(connectionToClient, value);
        }

        private void PimPamPumResponseButton()
        {
            PlayerView.EnableTakeHitButton(false);
            PlayerView.EnableBarrelButton(false);
        }

        protected virtual void UseCardState(int index, int player, Drop drop, int cardIndex)
        {
            switch (State)
            {
                case State.Play:
                case State.Dying:
                    if (player > -1)
                    {
                        StartCoroutine(PlayCard(player, drop, cardIndex));
                    }
                    break;
                case State.Discard:
                    if (drop == Drop.Board)
                    {
                        StartCoroutine(DiscardCardEndTurn(index));
                    }
                    break;
                case State.Duel:
                    if (drop == Drop.Board)
                    {
                        StartCoroutine(DuelResponse(index));
                    }
                    break;
                case State.Response:
                    if (drop == Drop.Board)
                    {
                        StartCoroutine(Response(index));
                    }
                    break;
            }
        }

        protected virtual void OnSetLocalPlayer() { }

        [Client]
        public void UseSkillClient()
        {
            CmdUseSkill();
        }

        [Client]
        public void PhaseOneDecision(Decision option, int index = -1, Drop dropEnum = Drop.Nothing, int property = -1)
        {
            CmdPhaseOneOption(option, index, dropEnum, property);
        }

        [Client]
        public void ChooseCard(int index)
        {
            CmdChooseCard(index);
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
            PimPamPumResponseButton();
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
            PimPamPumResponseButton();
            CmdMakeDecision(Decision.Barrel);
        }

        [Client]
        public void EndTurnButton()
        {
            PlayerView.EnableEndTurnButton(false);
            CmdEndTurn();
        }

        [Client]
        public void PassButton()
        {
            PlayerView.EnablePassButton(false);
            CmdMakeDecision(Decision.Skip);
        }

        [Server]
        protected virtual void UseSkill() { }

        [Command]
        private void CmdChooseCard(int choice)
        {
            WaitFor.CurrentWaitFor.MakeDecision(choice);
        }

        [Command]
        private void CmdMakeDecision(Decision decision)
        {
            MakeDecisionServer(decision);
        }

        [Command]
        private void CmdPhaseOneOption(Decision option, int player, Drop dropEnum, int card)
        {
            WaitFor.CurrentWaitFor.MakeDecision(option, player, dropEnum, card);
        }

        [Command]
        private void CmdBeginCardDrag(int index)
        {
            DraggedCardIndex = index;
            switch (State)
            {
                case State.Play:
                case State.Dying:
                    Hand[DraggedCardIndex].BeginCardDrag(this);
                    break;
                default:
                    GameController.Instance.HighlightTrash(PlayerNumber, true);
                    break;
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
            UseCardState(index, player, drop, cardIndex);
            GameController.Instance.StopTargeting(PlayerNumber);
        }

        [Command]
        private void CmdSetPlayerName(string name)
        {
            playerName = name;
            GameController.Instance.SetPlayerNames(PlayerNumber);
        }

        [Command]
        private void CmdUseSkill()
        {
            UseSkill();
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
            PlayerView.AddHandCard();
        }

        [ClientRpc]
        private void RpcRemoveCard()
        {
            if (isLocalPlayer) return;
            PlayerView.RemoveHandCard();
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
        public virtual void TargetSetLocalPlayer(NetworkConnection conn, int maxPlayers)
        {
            LocalPlayer = this;
            GameController.Instance.MaxPlayers = maxPlayers;
            GameController.Instance.SetPlayerViews();
            PlayerView = GameController.Instance.GetPlayerView(0);
            PlayerView.SetLocalPlayer();
            PlayerName = NetworkManagerButton.PlayerName;
            PlayerName = ToString();
            PlayerView.SetPlayerName(PlayerName);
            OnSetLocalPlayer();
        }

        [TargetRpc]
        private void TargetSetTargetable(NetworkConnection conn, bool value)
        {
            PlayerView.Droppable = value;
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
        private void TargetClickable(NetworkConnection conn, bool value)
        {
            PlayerView.EnableClick(value);
        }

        [TargetRpc]
        private void TargetClickableHand(NetworkConnection conn, bool value)
        {
            PlayerView.EnableClickHand(value);
        }

        [TargetRpc]
        private void TargetClickableProperties(NetworkConnection conn, bool value, bool weapon)
        {
            PlayerView.EnableClickProperties(value, weapon);
        }

        [TargetRpc]
        private void TargetAddCard(NetworkConnection conn, int index, CardStruct cs)
        {
            PlayerView.AddHandCard(index, cs);
        }

        [TargetRpc]
        private void TargetRemoveCard(NetworkConnection conn, int index)
        {
            PlayerView.RemoveHandCard(index);
        }

        [TargetRpc]
        private void TargetSetRole(NetworkConnection conn, Role role)
        {
            PlayerView.SetRole(role);
        }

        [TargetRpc]
        private void TargetSetup(NetworkConnection conn, int playerNumber)
        {
            if (!isLocalPlayer)
            {
                PlayerView = GameController.Instance.GetPlayerView(playerNumber, PlayerNumber);
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
        private void TargetEnablePassButton(NetworkConnection conn, bool value)
        {
            PlayerView.EnablePassButton(value);
        }

        [TargetRpc]
        private void TargetEnableBarrelButton(NetworkConnection conn, bool value)
        {
            PlayerView.EnableBarrelButton(value);
        }

        [TargetRpc]
        private void TargetEnableConfirmButton(NetworkConnection conn, bool value)
        {
            PlayerView.EnableConfirmButton(value);
        }

        [TargetRpc]
        private void TargetEnableCancelButton(NetworkConnection conn, bool value)
        {
            PlayerView.EnableCancelButton(value);
        }

        [TargetRpc]
        private void TargetEnableSkill(NetworkConnection conn, bool value)
        {
            PlayerView.EnablePlayerSkill(value);
        }

        [TargetRpc]
        private void TargetSetSkillStatus(NetworkConnection conn, bool value)
        {
            PlayerView.SetPlayerSkillStatus(value);
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