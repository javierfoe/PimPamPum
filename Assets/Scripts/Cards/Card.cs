using System.Collections;
using UnityEngine;

namespace PimPamPum
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
            get; private set;
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
}