using System.Collections;
using UnityEngine;

namespace PimPamPum
{
    public abstract class Card
    {
        private static Color brownCard = Color.red;

        public virtual Suit Suit => Struct.suit;

        public virtual Rank Rank => Struct.rank;

        public bool IsRed => Suit == Suit.Hearts || Suit == Suit.Diamonds;

        public CardStruct Struct
        {
            get; protected set;
        }

        public virtual Card Original => null;

        public virtual Color Color => brownCard;

        public virtual bool Is<T>() where T : Card, new()
        {
            return this is T;
        }

        protected void SetSuitRank(Suit suit = Suit.Null, Rank rank = Rank.Null)
        {
            Struct = new CardStruct
            {
                suit = suit,
                rank = rank,
                name = ToString(),
                color = Color
            };
        }

        public virtual void BeginCardDrag(PlayerController pc)
        {
            pc.BeginCardDrag(this);
        }

        public virtual IEnumerator PlayCard(PlayerController pc, int player, Drop drop, int cardIndex)
        {
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
            yield return GameController.Instance.PimPamPumEventPlayedCard(pc.PlayerNumber, player, this, drop, cardIndex);
        }

        public Card ConvertTo<T>() where T : Card, new()
        {
            Card converted = new T();
            return new ConvertedCard(this, converted);
        }

        public static Card CreateNew<T>(Suit suit, Rank rank) where T : Card, new()
        {
            Card res = new T();
            res.SetSuitRank(suit, rank);
            return res;
        }
    }
}