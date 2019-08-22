using UnityEngine;

namespace PimPamPum
{

    public class ConvertedCard : Card
    {
        private Card original, converted;

        public override Card Original => original;
        public override bool Is<T>() => converted.Is<T>();
        public override Suit Suit => Original.Suit;
        public override Rank Rank => Original.Rank;
        public override Color Color => converted.Color;

        public ConvertedCard(Card original, Card converted)
        {
            this.original = original;
            this.converted = converted;
        }

    }
}