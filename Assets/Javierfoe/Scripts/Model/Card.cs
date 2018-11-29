using UnityEngine;

namespace Bang
{

    public abstract class Card
    {
        public ESuit Suit
        {
            get; private set;
        }
        public ERank Rank
        {
            get; private set;
        }

        public Color Color
        {
            get
            {
                return this is Property ? Color.blue : Color.red;
            }
        }

        public Card(ESuit suit, ERank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public abstract void BeginCardDrag(PlayerController pc);

        public virtual void PlayCard(PlayerController pc, int player, int drop)
        {
            Debug.Log("Card: " + ToString() + " Target: " + player + " Drop: " + drop);
        }

    }

    public class Bang : Card
    {
        private Bang(ESuit suit, ERank rank) : base(suit, rank) { }

        public static Bang CreateBang(int index)
        {
            switch (index)
            {
                case 0:
                    return new Bang(ESuit.Spades, ERank.Ace);
                case 1:
                    return new Bang(ESuit.Hearts, ERank.Ace);
                case 2:
                    return new Bang(ESuit.Hearts, ERank.Queen);
                case 3:
                    return new Bang(ESuit.Hearts, ERank.King);
                case 4:
                    return new Bang(ESuit.Clubs, ERank.Two);
                case 5:
                    return new Bang(ESuit.Clubs, ERank.Three);
                case 6:
                    return new Bang(ESuit.Clubs, ERank.Four);
                case 7:
                    return new Bang(ESuit.Clubs, ERank.Five);
                case 8:
                    return new Bang(ESuit.Clubs, ERank.Six);
                case 9:
                    return new Bang(ESuit.Clubs, ERank.Seven);
                case 10:
                    return new Bang(ESuit.Clubs, ERank.Eight);
                case 11:
                    return new Bang(ESuit.Clubs, ERank.Nine);
                case 12:
                    return new Bang(ESuit.Diamonds, ERank.Ace);
                case 13:
                    return new Bang(ESuit.Diamonds, ERank.Two);
                case 14:
                    return new Bang(ESuit.Diamonds, ERank.Three);
                case 15:
                    return new Bang(ESuit.Diamonds, ERank.Four);
                case 16:
                    return new Bang(ESuit.Diamonds, ERank.Five);
                case 17:
                    return new Bang(ESuit.Diamonds, ERank.Six);
                case 18:
                    return new Bang(ESuit.Diamonds, ERank.Seven);
                case 19:
                    return new Bang(ESuit.Diamonds, ERank.Eight);
                case 20:
                    return new Bang(ESuit.Diamonds, ERank.Nine);
                case 21:
                    return new Bang(ESuit.Diamonds, ERank.Ten);
                case 22:
                    return new Bang(ESuit.Diamonds, ERank.Jack);
                case 23:
                    return new Bang(ESuit.Diamonds, ERank.Queen);
                case 24:
                    return new Bang(ESuit.Diamonds, ERank.King);
            }
            return null;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.BangBeginCardDrag();
        }

        public override void PlayCard(PlayerController pc, int player, int drop)
        {
            base.PlayCard(pc, player, drop);
            pc.ShotBang(player);
        }

        public override string ToString()
        {
            return "Bang";
        }
    }

    public class Missed : Card
    {
        private Missed(ESuit suit, ERank rank) : base(suit, rank) { }

        public static Missed CreateMissed(int index)
        {
            switch (index)
            {
                case 0:
                    return new Missed(ESuit.Spades, ERank.Two);
                case 1:
                    return new Missed(ESuit.Spades, ERank.Three);
                case 2:
                    return new Missed(ESuit.Spades, ERank.Four);
                case 3:
                    return new Missed(ESuit.Spades, ERank.Five);
                case 4:
                    return new Missed(ESuit.Spades, ERank.Six);
                case 5:
                    return new Missed(ESuit.Spades, ERank.Seven);
                case 6:
                    return new Missed(ESuit.Spades, ERank.Eight);
                case 7:
                    return new Missed(ESuit.Clubs, ERank.Ace);
                case 8:
                    return new Missed(ESuit.Clubs, ERank.Ten);
                case 9:
                    return new Missed(ESuit.Clubs, ERank.Jack);
                case 10:
                    return new Missed(ESuit.Clubs, ERank.Queen);
                case 11:
                    return new Missed(ESuit.Clubs, ERank.King);
            }
            return null;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "Missed";
        }
    }

    public class Indians : Card
    {
        private Indians(ESuit suit, ERank rank) : base(suit, rank) { }

        public static Indians CreateIndians(int index)
        {
            switch (index)
            {
                case 0:
                    return new Indians(ESuit.Diamonds, ERank.King);
                case 1:
                    return new Indians(ESuit.Diamonds, ERank.Ace);
            }
            return null;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetCard();
        }

        public override string ToString()
        {
            return "Indians";
        }
    }

    public class Duel : Card
    {
        private Duel(ESuit suit, ERank rank) : base(suit, rank) { }

        public static Duel CreateDuel(int index)
        {
            switch (index)
            {
                case 0:
                    return new Duel(ESuit.Clubs, ERank.Eight);
                case 1:
                    return new Duel(ESuit.Spades, ERank.Jack);
                case 2:
                    return new Duel(ESuit.Diamonds, ERank.Queen);
            }
            return null;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.TargetOthers();
        }

        public override string ToString()
        {
            return "Duel";
        }
    }

    public class CatBalou : Card
    {
        protected CatBalou(ESuit suit, ERank rank) : base(suit, rank) { }

        public static CatBalou CreateCatBalou(int index)
        {
            switch (index)
            {
                case 0:
                    return new CatBalou(ESuit.Diamonds, ERank.Nine);
                case 1:
                    return new CatBalou(ESuit.Diamonds, ERank.Ten);
                case 2:
                    return new CatBalou(ESuit.Diamonds, ERank.Jack);
                case 3:
                    return new CatBalou(ESuit.Hearts, ERank.King);
            }
            return null;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.CatBalouBeginCardDrag();
        }

        public override string ToString()
        {
            return "Cat Balou";
        }
    }

    public class Panic : CatBalou
    {
        private Panic(ESuit suit, ERank rank) : base(suit, rank) { }

        public static Panic CreatePanic(int index)
        {
            switch (index)
            {
                case 0:
                    return new Panic(ESuit.Hearts, ERank.Jack);
                case 1:
                    return new Panic(ESuit.Hearts, ERank.Queen);
                case 2:
                    return new Panic(ESuit.Hearts, ERank.Ace);
                case 3:
                    return new Panic(ESuit.Diamonds, ERank.Eight);
            }
            return null;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.PanicBeginCardDrag();
        }

        public override string ToString()
        {
            return "Panic";
        }
    }

    public class Gatling : Card
    {
        public Gatling() : base(ESuit.Hearts, ERank.Ten) { }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetCard();
        }

        public override string ToString()
        {
            return "Gatling";
        }
    }

    public class Beer : Card
    {
        private Beer(ERank rank) : base(ESuit.Hearts, rank) { }

        public static Beer CreateBeer(int index)
        {
            switch (index)
            {
                case 0:
                    return new Beer(ERank.Six);
                case 1:
                    return new Beer(ERank.Seven);
                case 2:
                    return new Beer(ERank.Eight);
                case 3:
                    return new Beer(ERank.Nine);
                case 4:
                    return new Beer(ERank.Ten);
                case 5:
                    return new Beer(ERank.Jack);
            }
            return null;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetCard();
        }

        public override void PlayCard(PlayerController pc, int player, int drop)
        {
            base.PlayCard(pc, player, drop);
            pc.DiscardCardUsed();
            pc.Heal();
            Debug.Log("Beer used.");
        }

        public override string ToString()
        {
            return "Beer";
        }
    }

    public class Saloon : Card
    {
        public Saloon() : base(ESuit.Hearts, ERank.Five) { }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetCard();
        }

        public override string ToString()
        {
            return "Saloon";
        }
    }

    public class GeneralStore : Card
    {
        private GeneralStore(ESuit suit, ERank rank) : base(suit, rank) { }

        public static GeneralStore CreateGeneralStore(int index)
        {
            switch (index)
            {
                case 0:
                    return new GeneralStore(ESuit.Spades, ERank.Queen);
                case 1:
                    return new GeneralStore(ESuit.Clubs, ERank.Nine);
            }
            return null;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetCard();
        }

        public override string ToString()
        {
            return "General Store";
        }
    }

    public abstract class Property : Card
    {
        protected Property(ESuit suit, ERank rank) : base(suit, rank) { }

        public abstract override void BeginCardDrag(PlayerController pc);

        public override void PlayCard(PlayerController pc, int player, int drop)
        {
            base.PlayCard(pc, player, drop);
            pc.UnequipDraggedCard();
            EquipProperty(pc, player, drop);
        }

        public virtual void EquipProperty(PlayerController pc, int player = -1, int drop = -1)
        {
            pc.EquipProperty(this);
        }

        public virtual void UnequipProperty(PlayerController pc) { }
    }

    public class Mustang : Property
    {
        private Mustang(ERank rank) : base(ESuit.Hearts, rank) { }

        public static Mustang CreateMustang(int index)
        {
            switch (index)
            {
                case 0:
                    return new Mustang(ERank.Eight);
                case 1:
                    return new Mustang(ERank.Nine);
            }
            return null;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetPropertyCard<Mustang>();
        }

        public override void EquipProperty(PlayerController pc, int player = -1, int drop = -1)
        {
            base.EquipProperty(pc, player, drop);
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
        private Barrel(ERank rank) : base(ESuit.Spades, rank) { }

        public static Barrel CreateBarrel(int index)
        {
            switch (index)
            {
                case 0:
                    return new Barrel(ERank.Queen);
                case 1:
                    return new Barrel(ERank.King);
            }
            return null;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetPropertyCard<Barrel>();
        }

        public override string ToString()
        {
            return "Barrel";
        }
    }

    public class Scope : Property
    {
        public Scope() : base(ESuit.Spades, ERank.Ace) { }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetPropertyCard<Scope>();
        }

        public override void EquipProperty(PlayerController pc, int player = -1, int drop = -1)
        {
            base.EquipProperty(pc, player, drop);
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

    public abstract class NegativeProperty : Property
    {
        protected ESuit Trigger
        {
            get; private set;
        }

        protected ERank Minimum
        {
            get; private set;
        }

        protected ERank Maximum
        {
            get; private set;
        }

        protected NegativeProperty(ESuit suit, ERank rank, ESuit trigger, ERank minimum, ERank maximum) : base(suit, rank)
        {
            Trigger = trigger;
            Minimum = minimum;
            Maximum = maximum;
        }

        protected NegativeProperty(ESuit suit, ERank rank, ESuit trigger) : this(suit, rank, trigger, ERank.Ace, ERank.Ace) { }

        public abstract bool CheckCondition(Card c);
    }

    public class Dynamite : NegativeProperty
    {
        public Dynamite() : base(ESuit.Hearts, ERank.Two, ESuit.Spades, ERank.Two, ERank.Nine) { }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetPropertyCard<Dynamite>();
        }

        public override void EquipProperty(PlayerController pc, int player = -1, int drop = -1)
        {
            base.EquipProperty(pc, player, drop);
            pc.EquipDynamite();
        }

        public override void UnequipProperty(PlayerController pc)
        {
            pc.UnequipDynamite();
        }

        public override bool CheckCondition(Card c)
        {
            bool res = c.Suit == Trigger && c.Rank <= Maximum && c.Rank >= Minimum;
            Debug.Log(res ? "BOOOOOM! A pastar!" : "Toma patatita caliente");
            return res;
        }

        public override string ToString()
        {
            return "Dynamite";
        }
    }

    public class Jail : NegativeProperty
    {
        private Jail(ESuit suit, ERank rank) : base(suit, rank, ESuit.Hearts) { }

        public static Jail CreateJail(int index)
        {
            switch (index)
            {
                case 0:
                    return new Jail(ESuit.Spades, ERank.Ten);
                case 1:
                    return new Jail(ESuit.Spades, ERank.Jack);
                case 2:
                    return new Jail(ESuit.Hearts, ERank.Four);
            }
            return null;
        }

        public override bool CheckCondition(Card c)
        {
            bool res = c.Suit == Trigger;
            Debug.Log(res ? "Me libro de la carcel" : "Hijoeputa malparido gonorrea, sigo en la carcel");
            return res;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.JailBeginCardDrag();
        }

        public override void PlayCard(PlayerController pc, int player, int drop)
        {
            pc.UnequipDraggedCard();
            pc.Imprison(player, this);
        }

        public override void EquipProperty(PlayerController pc, int player = -1, int drop = -1)
        {
            base.EquipProperty(pc, player, drop);
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

        protected Weapon(int range, ESuit suit, ERank rank) : base(suit, rank)
        {
            Range = range;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetCard();
        }

        public override void PlayCard(PlayerController pc, int player, int drop)
        {
            base.PlayCard(pc, player, drop);
            pc.EquipWeapon(this);
        }

        public override void EquipProperty(PlayerController pc, int player = -1, int drop = -1) { }

        public virtual void Bang(PlayerController pc)
        {
            pc.Bang();
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
        private Volcanic(ESuit suit) : base(1, suit, ERank.Ten) { }

        public static Volcanic CreateVolcanic(int index)
        {
            switch (index)
            {
                case 0:
                    return new Volcanic(ESuit.Clubs);
                case 1:
                    return new Volcanic(ESuit.Spades);
            }
            return null;
        }

        public override void Bang(PlayerController pc) { }

        public override string ToString()
        {
            return "Volcanic";
        }

    }

    public class Remington : Weapon
    {
        public Remington() : base(3, ESuit.Clubs, ERank.King) { }

        public override string ToString()
        {
            return "Remington";
        }
    }

    public class Schofield : Weapon
    {
        protected Schofield(ESuit suit, ERank rank) : base(2, suit, rank) { }

        public static Schofield CreateSchofield(int index)
        {
            switch (index)
            {
                case 0:
                    return new Schofield(ESuit.Clubs, ERank.Jack);
                case 1:
                    return new Schofield(ESuit.Clubs, ERank.Queen);
                case 2:
                    return new Schofield(ESuit.Spades, ERank.King);
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
        public Carabine() : base(4, ESuit.Clubs, ERank.Ace) { }

        public override string ToString()
        {
            return "Carabine";
        }
    }

    public class Winchester : Weapon
    {
        public Winchester() : base(5, ESuit.Spades, ERank.Eight) { }

        public override string ToString()
        {
            return "Winchester";
        }
    }

    public abstract class Draw : Card
    {
        private int numberToDraw;
        protected Draw(int numberToDraw, ESuit suit, ERank rank) : base(suit, rank)
        {
            this.numberToDraw = numberToDraw;
        }

        public override void PlayCard(PlayerController pc, int player, int drop)
        {
            base.PlayCard(pc, player, drop);
            pc.Draw(numberToDraw);
            pc.DiscardCardUsed();
            pc.FinishCardUsed();
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            pc.SelfTargetCard();
        }
    }

    public class WellsFargo : Draw
    {
        public WellsFargo() : base(3, ESuit.Hearts, ERank.Three) { }

        public override string ToString()
        {
            return "Wells Fargo";
        }
    }

    public class Stagecoach : Draw
    {
        public Stagecoach() : base(2, ESuit.Spades, ERank.Nine) { }

        public override string ToString()
        {
            return "Stagecoach";
        }
    }

}
