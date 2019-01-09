using System.Collections;
using UnityEngine;

namespace Bang
{

    public abstract class Card
    {
        public Suit Suit
        {
            get; private set;
        }
        public Rank Rank
        {
            get; private set;
        }

        public CardStruct Struct
        {
            get
            {
                return new CardStruct
                {
                    color = Color,
                    name = ToString(),
                    suit = Suit,
                    rank = Rank
                };
            }
        }

        private Color Color
        {
            get
            {
                return this is Property ? Color.blue : Color.red;
            }
        }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
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
            Card res = CreateNew<T>();
            res.Suit = Suit;
            res.Rank = Rank;
            return res;
        }

        private static T CreateNew<T>() where T: Card, new()
        {
            return new T();
        }

    }

    public class Bang : Card
    {
        private Bang(Suit suit, Rank rank) : base(suit, rank) { }

        public static Bang CreateBang(int index)
        {
            switch (index)
            {
                case 0:
                    return new Bang(Suit.Spades, Rank.Ace);
                case 1:
                    return new Bang(Suit.Hearts, Rank.Ace);
                case 2:
                    return new Bang(Suit.Hearts, Rank.Queen);
                case 3:
                    return new Bang(Suit.Hearts, Rank.King);
                case 4:
                    return new Bang(Suit.Clubs, Rank.Two);
                case 5:
                    return new Bang(Suit.Clubs, Rank.Three);
                case 6:
                    return new Bang(Suit.Clubs, Rank.Four);
                case 7:
                    return new Bang(Suit.Clubs, Rank.Five);
                case 8:
                    return new Bang(Suit.Clubs, Rank.Six);
                case 9:
                    return new Bang(Suit.Clubs, Rank.Seven);
                case 10:
                    return new Bang(Suit.Clubs, Rank.Eight);
                case 11:
                    return new Bang(Suit.Clubs, Rank.Nine);
                case 12:
                    return new Bang(Suit.Diamonds, Rank.Ace);
                case 13:
                    return new Bang(Suit.Diamonds, Rank.Two);
                case 14:
                    return new Bang(Suit.Diamonds, Rank.Three);
                case 15:
                    return new Bang(Suit.Diamonds, Rank.Four);
                case 16:
                    return new Bang(Suit.Diamonds, Rank.Five);
                case 17:
                    return new Bang(Suit.Diamonds, Rank.Six);
                case 18:
                    return new Bang(Suit.Diamonds, Rank.Seven);
                case 19:
                    return new Bang(Suit.Diamonds, Rank.Eight);
                case 20:
                    return new Bang(Suit.Diamonds, Rank.Nine);
                case 21:
                    return new Bang(Suit.Diamonds, Rank.Ten);
                case 22:
                    return new Bang(Suit.Diamonds, Rank.Jack);
                case 23:
                    return new Bang(Suit.Diamonds, Rank.Queen);
                case 24:
                    return new Bang(Suit.Diamonds, Rank.King);
            }
            return null;
        }

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
        private Missed(Suit suit, Rank rank) : base(suit, rank) { }

        public static Missed CreateMissed(int index)
        {
            switch (index)
            {
                case 0:
                    return new Missed(Suit.Spades, Rank.Two);
                case 1:
                    return new Missed(Suit.Spades, Rank.Three);
                case 2:
                    return new Missed(Suit.Spades, Rank.Four);
                case 3:
                    return new Missed(Suit.Spades, Rank.Five);
                case 4:
                    return new Missed(Suit.Spades, Rank.Six);
                case 5:
                    return new Missed(Suit.Spades, Rank.Seven);
                case 6:
                    return new Missed(Suit.Spades, Rank.Eight);
                case 7:
                    return new Missed(Suit.Clubs, Rank.Ace);
                case 8:
                    return new Missed(Suit.Clubs, Rank.Ten);
                case 9:
                    return new Missed(Suit.Clubs, Rank.Jack);
                case 10:
                    return new Missed(Suit.Clubs, Rank.Queen);
                case 11:
                    return new Missed(Suit.Clubs, Rank.King);
            }
            return null;
        }

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
        private Indians(Suit suit, Rank rank) : base(suit, rank) { }

        public static Indians CreateIndians(int index)
        {
            switch (index)
            {
                case 0:
                    return new Indians(Suit.Diamonds, Rank.King);
                case 1:
                    return new Indians(Suit.Diamonds, Rank.Ace);
            }
            return null;
        }

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
        private Duel(Suit suit, Rank rank) : base(suit, rank) { }

        public static Duel CreateDuel(int index)
        {
            switch (index)
            {
                case 0:
                    return new Duel(Suit.Clubs, Rank.Eight);
                case 1:
                    return new Duel(Suit.Spades, Rank.Jack);
                case 2:
                    return new Duel(Suit.Diamonds, Rank.Queen);
            }
            return null;
        }

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
        protected CatBalou(Suit suit, Rank rank) : base(suit, rank) { }

        public static CatBalou CreateCatBalou(int index)
        {
            switch (index)
            {
                case 0:
                    return new CatBalou(Suit.Diamonds, Rank.Nine);
                case 1:
                    return new CatBalou(Suit.Diamonds, Rank.Ten);
                case 2:
                    return new CatBalou(Suit.Diamonds, Rank.Jack);
                case 3:
                    return new CatBalou(Suit.Hearts, Rank.King);
            }
            return null;
        }

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
        private Panic(Suit suit, Rank rank) : base(suit, rank) { }

        public static Panic CreatePanic(int index)
        {
            switch (index)
            {
                case 0:
                    return new Panic(Suit.Hearts, Rank.Jack);
                case 1:
                    return new Panic(Suit.Hearts, Rank.Queen);
                case 2:
                    return new Panic(Suit.Hearts, Rank.Ace);
                case 3:
                    return new Panic(Suit.Diamonds, Rank.Eight);
            }
            return null;
        }

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
        public Gatling() : base(Suit.Hearts, Rank.Ten) { }

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
        private Beer(Rank rank) : base(Suit.Hearts, rank) { }

        public static Beer CreateBeer(int index)
        {
            switch (index)
            {
                case 0:
                    return new Beer(Rank.Six);
                case 1:
                    return new Beer(Rank.Seven);
                case 2:
                    return new Beer(Rank.Eight);
                case 3:
                    return new Beer(Rank.Nine);
                case 4:
                    return new Beer(Rank.Ten);
                case 5:
                    return new Beer(Rank.Jack);
            }
            return null;
        }

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
        public Saloon() : base(Suit.Hearts, Rank.Five) { }

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
        private GeneralStore(Suit suit, Rank rank) : base(suit, rank) { }

        public static GeneralStore CreateGeneralStore(int index)
        {
            switch (index)
            {
                case 0:
                    return new GeneralStore(Suit.Spades, Rank.Queen);
                case 1:
                    return new GeneralStore(Suit.Clubs, Rank.Nine);
            }
            return null;
        }

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
        protected Property(Suit suit, Rank rank) : base(suit, rank) { }

        public abstract override void BeginCardDrag(PlayerController pc);

        protected override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            pc.UnequipDraggedCard();
            pc.EquipPropertyTo(player, this);
            yield return null;
        }

        public virtual void EquipProperty(PlayerController pc)
        {
            pc.EquipProperty(this);
        }

        public virtual void UnequipProperty(PlayerController pc) { }
    }

    public class Mustang : Property
    {
        private Mustang(Rank rank) : base(Suit.Hearts, rank) { }

        public static Mustang CreateMustang(int index)
        {
            switch (index)
            {
                case 0:
                    return new Mustang(Rank.Eight);
                case 1:
                    return new Mustang(Rank.Nine);
            }
            return null;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetPropertyCard<Mustang>();
        }

        public override void EquipProperty(PlayerController pc)
        {
            base.EquipProperty(pc);
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
        private Barrel(Rank rank) : base(Suit.Spades, rank) { }

        public static Barrel CreateBarrel(int index)
        {
            switch (index)
            {
                case 0:
                    return new Barrel(Rank.Queen);
                case 1:
                    return new Barrel(Rank.King);
            }
            return null;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetPropertyCard<Barrel>();
        }

        public override void EquipProperty(PlayerController pc)
        {
            base.EquipProperty(pc);
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
        public Scope() : base(Suit.Spades, Rank.Ace) { }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetPropertyCard<Scope>();
        }

        public override void EquipProperty(PlayerController pc)
        {
            base.EquipProperty(pc);
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
        public Dynamite() : base(Suit.Hearts, Rank.Two) { }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetPropertyCard<Dynamite>();
        }

        public override void EquipProperty(PlayerController pc)
        {
            base.EquipProperty(pc);
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
        private Jail(Suit suit, Rank rank) : base(suit, rank) { }

        public static Jail CreateJail(int index)
        {
            switch (index)
            {
                case 0:
                    return new Jail(Suit.Spades, Rank.Ten);
                case 1:
                    return new Jail(Suit.Spades, Rank.Jack);
                case 2:
                    return new Jail(Suit.Hearts, Rank.Four);
            }
            return null;
        }

        public static bool CheckCondition(Card c)
        {
            return c.Suit == Suit.Hearts;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.JailBeginCardDrag();
        }

        public override void EquipProperty(PlayerController pc)
        {
            base.EquipProperty(pc);
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

        protected Weapon(int range) : this(range, 0, 0) { }

        protected Weapon(int range, Suit suit, Rank rank) : base(suit, rank)
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
        private Volcanic(Suit suit) : base(1, suit, Rank.Ten) { }

        public static Volcanic CreateVolcanic(int index)
        {
            switch (index)
            {
                case 0:
                    return new Volcanic(Suit.Clubs);
                case 1:
                    return new Volcanic(Suit.Spades);
            }
            return null;
        }

        public override bool Bang(PlayerController pc) { return true; }

        public override string ToString()
        {
            return "Volcanic";
        }

    }

    public class Remington : Weapon
    {
        public Remington() : base(3, Suit.Clubs, Rank.King) { }

        public override string ToString()
        {
            return "Remington";
        }
    }

    public class Schofield : Weapon
    {
        protected Schofield(Suit suit, Rank rank) : base(2, suit, rank) { }

        public static Schofield CreateSchofield(int index)
        {
            switch (index)
            {
                case 0:
                    return new Schofield(Suit.Clubs, Rank.Jack);
                case 1:
                    return new Schofield(Suit.Clubs, Rank.Queen);
                case 2:
                    return new Schofield(Suit.Spades, Rank.King);
            }
            return null;
        }

        public override string ToString()
        {
            return "Schofield";
        }
    }

    public class Carabine : Weapon
    {
        public Carabine() : base(4, Suit.Clubs, Rank.Ace) { }

        public override string ToString()
        {
            return "Carabine";
        }
    }

    public class Winchester : Weapon
    {
        public Winchester() : base(5, Suit.Spades, Rank.Eight) { }

        public override string ToString()
        {
            return "Winchester";
        }
    }

    public abstract class Draw : Card
    {
        private int numberToDraw;
        protected Draw(int numberToDraw, Suit suit, Rank rank) : base(suit, rank)
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
        public WellsFargo() : base(3, Suit.Hearts, Rank.Three) { }

        public override string ToString()
        {
            return "Wells Fargo";
        }
    }

    public class Stagecoach : Draw
    {
        public Stagecoach() : base(2, Suit.Spades, Rank.Nine) { }

        public override string ToString()
        {
            return "Stagecoach";
        }
    }

}
