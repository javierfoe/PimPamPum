using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PimPamPum
{
    public abstract class PlayerController : MonoBehaviour
    {
        public static PlayerController CurrentPlayableCharacter { get; private set; }

        private static Colt45 colt45 = new Colt45();

        [SerializeField] private Character character = Character.AnnieVersary;
        [SerializeField] private string characterName = "";
        [SerializeField] private int characterHP = 4;


        private int cardAmount;
        private CardValues weaponCard;
        protected int hp;

        private readonly List<CardValues> propertyCards = new List<CardValues>();

        private Weapon weapon;
        private List<Card> properties;
        private IPlayerView playerView;
        private bool endTurn, jail, dynamite;
        protected Card draggedCard;
        protected int pimPamPumsUsed;

        public int PlayerNumber { get; set; }
        public int WeaponRange => Weapon.Range + Scope;
        public bool Stealable => HasCards || HasProperties || !HasColt45;
        public bool HasCards => Hand.Count > 0;
        public bool HasProperties => properties.Count > 0;
        public bool HasColt45 => Weapon == colt45;
        public bool IsDying => HP < 1;
        protected bool ActivePlayer => GameController.CurrentPlayer == PlayerNumber;
        protected bool CanShoot => PimPamPum();

        public List<Card> Hand
        {
            get; protected set;
        }

        public State State
        {
            get; protected set;
        }

        public string PlayerName
        {
            get; set;
        }

        public int DraggedCardIndex
        {
            get; set;
        }

        public Character Character
        {
            get { return character; }
        }

        public IPlayerView PlayerView
        {
            get
            {
                return playerView;
            }
            protected set
            {
                playerView = value;
                playerView.PlayerIndex = PlayerNumber;
            }
        }

        public bool IsDead
        {
            get; private set;
        }

        protected int HP
        {
            get { return hp; }
            set
            {
                hp = value;
                PlayerView.UpdateHP(value);
            }
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
            get; set;
        }

        public Weapon Weapon
        {
            get { return weapon; }
            set
            {
                weapon = value;
                weaponCard = weapon.Struct;
                EquipWeaponCard(weaponCard);
            }
        }

        public Actions Actions
        {
            get; private set;
        }

        protected virtual void Awake()
        {
            State = State.OutOfTurn;
            BeerHeal = 1;
            Phase1CardsDrawn = 2;
            MissesToDodge = 1;
            DrawEffectCards = 1;
            MaxHP = characterHP;
            Hand = new List<Card>();
            properties = new List<Card>();
            Actions = GetComponent<Actions>();
        }

        public bool BelongsToTeam(Team team)
        {
            return
                (team == Team.Law && (Role == Role.Sheriff || Role == Role.Deputy)) ||
                (team == Team.Outlaw && Role == Role.Outlaw);
        }

        public virtual void Setup()
        {
            if (Role == Role.Sheriff)
            {
                MaxHP++;
            }
            else
            {
                PlayerView.SetRole(Role);
            }
            HP = MaxHP;
            PlayerView.SetCharacter(characterName);
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
            Draw(HP);
        }

        public void AddCard(Card c)
        {
            Hand.Add(c);
            Actions.AddCard(c.Struct);
            UpdateCards(cardAmount + 1);
        }

        public void AddHandCard(int index, CardValues card)
        {
            PlayerView.AddHandCard(index, card);
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

        private Card RemoveCardFromHand(int index)
        {
            Card card = Hand[index];
            Hand.RemoveAt(index);
            Actions.RemoveCard(index);
            UpdateCards(cardAmount - 1);
            return card;
        }

        public void RemoveHandCard(int index)
        {
            PlayerView.RemoveHandCard(index);
        }

        public void EnableCard(int index, bool enable)
        {
            PlayerView.EnableCard(index, enable);
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
            propertyCards.Add(c.Struct);
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
            if (Card.CheckCondition<Dynamite>(drawEffectCoroutine.DrawEffectCard))
            {
                yield return PimPamPumEvent(this + ": Dynamite exploded. 3 damage inflicted");
                GameController.DiscardCard(d);
                yield return GetHitBy(PimPamPumConstants.NoOne, 3);
            }
            else
            {
                yield return PimPamPumEvent(this + ": Avoids the dynamite and passes it to the next player");
                GameController.PassDynamite(PlayerNumber, d);
            }
        }

        public IEnumerator JailCheck()
        {
            DrawEffectCoroutine drawEffectCoroutine = new DrawEffectCoroutine(this);
            yield return drawEffectCoroutine;
            int index;
            Jail j = FindProperty<Jail>(out index);
            endTurn = !Card.CheckCondition<Jail>(drawEffectCoroutine.DrawEffectCard);
            UnequipProperty(index);
            yield return PimPamPumEvent(this + (endTurn ? " stays in prison." : " has escaped the prison. "));
            GameController.DiscardCard(j);
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

        public IEnumerator TurnTimeUp()
        {
            if (ActivePlayer)
            {
                int cardLimit = CardLimit();
                while (Hand.Count > cardLimit)
                {
                    yield return DiscardRandomCardEndTurn();
                }
            }
            ForceEndTurn();
        }

        public virtual void ForceEndTurn()
        {
            State = State.OutOfTurn;
            OriginalHand();
            DisableCards();
            SetTurn(false);
            GameController.EndTurn(PlayerNumber);
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

        protected void ConvertHandFrom<T>(Card c) where T : Card
        {
            Card auxC;
            int length = Hand.Count;
            for (int i = 0; i < length; i++)
            {
                auxC = Hand[i];
                if (auxC.Is<T>())
                {
                    Hand[i] = new ConvertedCard(auxC, c);
                }
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
                Actions.SetCard(i, false);
            }
            EnableSkill(false);
        }

        protected void EnableAllCards()
        {
            int length = Hand.Count;
            for (int i = 0; i < length; i++)
            {
                Actions.SetCard(i, true);
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
            bool pimPamPums = CanShoot;
            int length = Hand.Count;
            Card c;
            bool isMissed;
            bool isPimPamPum;
            for (int i = 0; i < length; i++)
            {
                c = Hand[i];
                isMissed = c.Is<Missed>();
                isPimPamPum = c.Is<PimPamPum>();
                Actions.SetCard(i, !isPimPamPum && !isMissed || !isMissed && pimPamPums);
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
                Actions.SetCard(i, Hand[i].Is<T>());
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

        protected IEnumerator Response(int index)
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
            MakeDecision(Decision.Avoid, index);
        }

        public IEnumerator DiscardRandomCardEndTurn()
        {
            int index = Random.Range(0, Hand.Count);
            yield return DiscardCard(index);
        }

        public IEnumerator DiscardCardEndTurn(int index)
        {
            yield return DiscardCard(index);
            EndTurn();
        }

        private IEnumerator DiscardCard(int index)
        {
            DisableCards();
            Card c = UnequipHandCard(index);
            yield return GameController.DiscardEffect(PlayerNumber, c);
        }

        public IEnumerator Saloon()
        {
            GameController.Saloon();
            yield return null;
        }

        public void PimPamPumUsed()
        {
            pimPamPumsUsed++;
        }

        public void TradeTwoForOne(int player)
        {
            GameController.TradeTwoForOne(PlayerNumber, DraggedCardIndex, player);
        }

        public IEnumerator ShootPimPamPum(int target)
        {
            yield return ShootPimPamPumTrigger(target);
        }

        protected virtual IEnumerator ShootPimPamPumTrigger(int target)
        {
            yield return GameController.PimPamPum(PlayerNumber, target, MissesToDodge);
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

        public virtual bool EndTurnDiscardPickup(int player)
        {
            return false;
        }

        public virtual bool PimPamPum()
        {
            return Weapon.PimPamPum() || pimPamPumsUsed < 1;
        }

        public void EquipWeapon(Weapon weapon)
        {
            if (!HasColt45)
            {
                GameController.DiscardCard(Weapon);
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

        public void SetStealable(bool value)
        {
            if (value && !Stealable) return;
            PlayerView.SetStealable(value, HasCards, !HasColt45);
        }

        public virtual void BeginCardDrag(Card c)
        {
            draggedCard = c;
            c.BeginCardDrag(this);
        }

        public void PimPamPumBeginCardDrag()
        {
            GameController.TargetPlayersRange(PlayerNumber, Weapon.Range + Scope, draggedCard);
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

        public void TargetOthersWithHand()
        {
            GameController.TargetOthersWithHand(PlayerNumber, draggedCard);
        }

        public void SelfTargetCard()
        {
            GameController.TargetSelf(PlayerNumber);
        }

        public void SelfTargetPropertyCard<T>() where T : Property, new()
        {
            GameController.TargetSelfProperty<T>(PlayerNumber);
        }

        public void StopTargeting()
        {
            SetStealable(false);
        }

        public virtual bool Immune(Card c)
        {
            return false;
        }

        public IEnumerator TurnTimer(WaitFor turnTimer)
        {
            yield return turnTimer;
            if (turnTimer.TimeUp)
            {
                yield return TurnTimeUp();
            }
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
                yield return GameController.PimPamPumEventHitBy(attacker, PlayerNumber);
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
            ResetDraggableCard();
            if (Role != Role.Sheriff) PlayerView.SetRole(Role);
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

            GameController.CheckDeath(deadCards);
            GameController.CheckMurder(killer, PlayerNumber);
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
            if (IsDying)
            {
                EnableCardsDying();
            }
            else if (ActivePlayer)
            {
                Phase2();
            }
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

        public virtual IEnumerator StolenBy(int thief)
        {
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
            Card card = RemoveCardFromHand(index);
            card = card.Original ?? card;
            return card;
        }

        public Card UnequipProperty(int index)
        {
            Property card = (Property)properties[index];
            properties.RemoveAt(index);
            propertyCards.RemoveAt(index);
            card.RemovePropertyEffect(this);
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
            Actions.TakeHit = value;
        }

        public void EnableEndTurnButton(bool value)
        {
            Actions.EndTurn = value;
        }

        public void EnableDieButton(bool value)
        {
            Actions.Die = value;
        }

        public void EnablePassButton(bool value)
        {
            Actions.Pass = value;
        }

        public void EnableClick(bool value)
        {
            PlayerView.EnableClick(value);
        }

        public void EnableClickHand(bool value)
        {
            PlayerView.EnableClickHand(value);
        }

        public void EnableClickProperties(bool value)
        {
            PlayerView.EnableClickProperties(value, !HasColt45);
        }

        public void MakeDecision(Decision decision, int index = -1)
        {
            DisableCards();
            Card card = index > -1 ? Hand[index] : null;
            WaitForController.MakeDecision(decision, card);
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

        public void SetTargetable(bool value)
        {
            PlayerView.Droppable = value;
        }

        public IEnumerator PimPamPumEvent(string pimPamPumEvent)
        {
            yield return GameController.PimPamPumEvent(pimPamPumEvent);
        }

        public override string ToString()
        {
            return PlayerName;
        }

        public virtual void EnableSkill(bool value)
        {
            Actions.PlayerSkillEnable = value;
        }

        public void SetSkill(bool value)
        {
            Actions.PlayerSkill = value;
        }

        public void EnableBarrelButton(bool value)
        {
            Actions.Barrel = value;
        }

        public void EnableConfirmButton(bool value)
        {
            Actions.Confirm = value;
        }

        public void EnableCancelButton(bool value)
        {
            Actions.Cancel = value;
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

        private void ResetDraggableCard()
        {
            CardView.CurrentCardView?.OnEndDrag();
        }

        protected virtual void OnSetLocalPlayer() { }

        public void UpdateCardsHost()
        {
            PlayerView.UpdateCards(Hand.Count);
        }

        private void UpdateCards(int cards)
        {
            cardAmount = cards;
            PlayerView.UpdateCards(cards);
        }

        private void SetTurn(bool value)
        {
            PlayerView.SetTurn(value);
        }

        private void EquipWeaponCard(CardValues cs)
        {
            PlayerView.EquipWeapon(cs);
        }

        public void PhaseOneDecision(Decision option, int index = -1, Drop dropEnum = Drop.Nothing, int property = -1)
        {
            WaitForController.MakeDecision(option, index, dropEnum, property);
        }

        public void ChooseCard(int index)
        {
            WaitForController.MakeDecision(index);
        }

        public void WillinglyDie()
        {
            PlayerView.EnableDieButton(false);
            MakeDecision(Decision.Die);
        }

        public void TakeHit()
        {
            PimPamPumResponseButton();
            MakeDecision(Decision.TakeHit);
        }

        public void BeginCardDrag(int index)
        {
            DraggedCardIndex = index;
            switch (State)
            {
                case State.Play:
                case State.Dying:
                    BeginCardDrag(Hand[DraggedCardIndex]);
                    break;
                default:
                    Actions.Thrash = true;
                    break;
            }
        }

        public void UseCard(int index, int player, Drop drop, int cardIndex)
        {
            UseCardState(index, player, drop, cardIndex);
            GameController.StopTargeting(PlayerNumber);
        }

        public void UseBarrel()
        {
            PimPamPumResponseButton();
            MakeDecision(Decision.Barrel);
        }

        public void EndTurnButton()
        {
            PlayerView.EnableEndTurnButton(false);
            EndTurn();
        }

        public void PassButton()
        {
            PlayerView.EnablePassButton(false);
            MakeDecision(Decision.Skip);
        }

        public virtual void UseSkill() { }
    }
}