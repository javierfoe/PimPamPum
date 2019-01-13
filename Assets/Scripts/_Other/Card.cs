using System.Collections;
using UnityEngine;

namespace Bang
{

    public abstract class Card
    {

        private static Color brownCard = Color.red;

        public Suit Suit
        {
            get
            {
                return Struct.suit;
            }
        }

        public Rank Rank
        {
            get
            {
                return Struct.rank;
            }
        }

        public bool IsRed
        {
            get
            {
                return Suit == Suit.Hearts || Suit == Suit.Diamonds;
            }
        }

        public CardStruct Struct
        {
            get; set;
        }

        public Card Original
        {
            get; private set;
        }

        protected virtual Color GetColor()
        {
            return brownCard;
        }

        public Card()
        {
            SetSuitRank(Suit.Null, Rank.Null);
        }

        private void SetSuitRank(Suit suit, Rank rank)
        {
            Struct = new CardStruct
            {
                suit = suit,
                rank = rank,
                name = ToString(),
                color = GetColor()
            };
        }

        public virtual void BeginCardDrag(PlayerController pc) { }

        public virtual IEnumerator PlayCard(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            pc.DisableCards();
            yield return CardEvent(pc, player, drop, cardIndex);
            yield return CardEffect(pc, player, drop, cardIndex);
            pc.FinishCardUsed();
        }

        protected virtual IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            pc.DiscardCardUsed();
            yield return null;
        }

        protected virtual IEnumerator CardEvent(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return pc.BangEventPlayedCard(this, player, drop, cardIndex);
        }

        public Card ConvertTo<T>() where T : Card, new()
        {
            Card res = new T
            {
                Struct = Struct,
                Original = this
            };
            return res;
        }

        public static Card CreateNew<T>(Suit suit, Rank rank) where T : Card, new()
        {
            Card res = new T();
            res.SetSuitRank(suit, rank);
            return res;
        }

    }

    public class Bang : Card
    {
        public override void BeginCardDrag(PlayerController pc)
        {
            pc.BangBeginCardDrag();
        }

        protected override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return base.CardEffect(pc, player, drop, cardIndex);
            yield return pc.ShotBang(player);
        }

        public override string ToString()
        {
            return "Bang";
        }
    }

    public class Missed : Card
    {
        protected override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return null;
        }

        public override string ToString()
        {
            return "Missed";
        }
    }

    public class Indians : Card
    {

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetCard();
        }

        protected override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return base.CardEffect(pc, player, drop, cardIndex);
            yield return pc.Indians();
        }

        public override string ToString()
        {
            return "Indians";
        }
    }

    public class Duel : Card
    {

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.TargetOthers();
        }

        protected override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return base.CardEffect(pc, player, drop, cardIndex);
            yield return pc.Duel(player);
        }

        public override string ToString()
        {
            return "Duel";
        }
    }

    public class CatBalou : Card
    {

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.CatBalouBeginCardDrag();
        }

        protected override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return StealCard(pc, player, drop, cardIndex);
        }

        protected virtual IEnumerator StealCard(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return pc.CatBalou(player, drop, cardIndex);
        }

        public override string ToString()
        {
            return "Cat Balou";
        }
    }

    public class Panic : CatBalou
    {

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.PanicBeginCardDrag();
        }

        protected override IEnumerator StealCard(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return pc.Panic(player, drop, cardIndex);
        }

        public override string ToString()
        {
            return "Panic";
        }
    }

    public class Gatling : Card
    {

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetCard();
        }

        protected override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return base.CardEffect(pc, player, drop, cardIndex);
            yield return pc.Gatling();
        }

        public override string ToString()
        {
            return "Gatling";
        }
    }

    public class Beer : Card
    {

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetCard();
        }

        protected override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return base.CardEffect(pc, player, drop, cardIndex);
            yield return pc.HealFromBeer();
        }

        public override string ToString()
        {
            return "Beer";
        }
    }

    public class Saloon : Card
    {

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetCard();
        }

        protected override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return base.CardEffect(pc, player, drop, cardIndex);
            yield return pc.Saloon();
        }

        public override string ToString()
        {
            return "Saloon";
        }
    }

    public class GeneralStore : Card
    {

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetCard();
        }

        protected override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return base.CardEffect(pc, player, drop, cardIndex);
            yield return pc.GeneralStore();
        }

        public override string ToString()
        {
            return "General Store";
        }
    }

    public abstract class Property : Card
    {
        private static Color property = Color.blue;

        protected override Color GetColor()
        {
            return property;
        }

        protected override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            pc.UnequipDraggedCard();
            pc.EquipPropertyTo(player, this);
            yield return null;
        }

        public virtual void EquipProperty(PlayerController pc)
        {
            pc.EquipProperty(this);
            AddPropertyEffect(pc);
        }

        public virtual void AddPropertyEffect(PlayerController pc) { }

        public virtual void UnequipProperty(PlayerController pc) { }
    }

    public class Mustang : Property
    {

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetPropertyCard<Mustang>();
        }

        public override void AddPropertyEffect(PlayerController pc)
        {
            pc.EquipMustang();
        }

        public override void UnequipProperty(PlayerController pc)
        {
            pc.UnequipMustang();
        }

        public override string ToString()
        {
            return "Mustang";
        }
    }

    public class Barrel : Property
    {

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetPropertyCard<Barrel>();
        }

        public override void AddPropertyEffect(PlayerController pc)
        {
            pc.EquipBarrel();
        }

        public override void UnequipProperty(PlayerController pc)
        {
            pc.UnequipBarrel();
        }

        public static bool CheckCondition(Card c)
        {
            return c.Suit == Suit.Hearts;
        }

        public override string ToString()
        {
            return "Barrel";
        }
    }

    public class Scope : Property
    {

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetPropertyCard<Scope>();
        }

        public override void AddPropertyEffect(PlayerController pc)
        {
            pc.EquipScope();
        }

        public override void UnequipProperty(PlayerController pc)
        {
            pc.UnequipScope();
        }

        public override string ToString()
        {
            return "Binoculars";
        }
    }

    public class Dynamite : Property
    {

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetPropertyCard<Dynamite>();
        }

        public override void AddPropertyEffect(PlayerController pc)
        {
            pc.EquipDynamite();
        }

        public override void UnequipProperty(PlayerController pc)
        {
            pc.UnequipDynamite();
        }

        public static bool CheckCondition(Card c)
        {
            return c.Suit == Suit.Spades && c.Rank <= Rank.Nine && c.Rank >= Rank.Two;
        }

        public override string ToString()
        {
            return "Dynamite";
        }
    }

    public class Jail : Property
    {

        public static bool CheckCondition(Card c)
        {
            return c.Suit == Suit.Hearts;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.JailBeginCardDrag();
        }

        public override void AddPropertyEffect(PlayerController pc)
        {
            pc.EquipJail();
        }

        public override void UnequipProperty(PlayerController pc)
        {
            pc.UnequipJail();
        }

        public override string ToString()
        {
            return "Jail";
        }
    }

    public abstract class Weapon : Property
    {
        public int Range
        {
            get; private set;
        }

        protected Weapon(int range)
        {
            Range = range;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetCard();
        }

        public override void EquipProperty(PlayerController pc)
        {
            pc.EquipWeapon(this);
        }

        public virtual bool Bang(PlayerController pc)
        {
            return pc.Bang();
        }
    }

    public class Colt45 : Weapon
    {
        public Colt45() : base(1) { }

        public override string ToString()
        {
            return "Colt45";
        }
    }

    public class Volcanic : Weapon
    {
        public Volcanic() : base(1) { }

        public override bool Bang(PlayerController pc) { return true; }

        public override string ToString()
        {
            return "Volcanic";
        }

    }

    public class Remington : Weapon
    {
        public Remington() : base(3) { }

        public override string ToString()
        {
            return "Remington";
        }
    }

    public class Schofield : Weapon
    {
        public Schofield() : base(2) { }

        public override string ToString()
        {
            return "Schofield";
        }
    }

    public class Carabine : Weapon
    {
        public Carabine() : base(4) { }

        public override string ToString()
        {
            return "Carabine";
        }
    }

    public class Winchester : Weapon
    {
        public Winchester() : base(5) { }

        public override string ToString()
        {
            return "Winchester";
        }
    }

    public abstract class Draw : Card
    {
        private int numberToDraw;

        protected Draw(int numberToDraw)
        {
            this.numberToDraw = numberToDraw;
        }

        protected override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return base.CardEffect(pc, player, drop, cardIndex);
            yield return pc.DrawFromCard(numberToDraw);
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetCard();
        }
    }

    public class WellsFargo : Draw
    {
        public WellsFargo() : base(3) { }

        public override string ToString()
        {
            return "Wells Fargo";
        }
    }

    public class Stagecoach : Draw
    {
        public Stagecoach() : base(2) { }

        public override string ToString()
        {
            return "Stagecoach";
        }
    }

}