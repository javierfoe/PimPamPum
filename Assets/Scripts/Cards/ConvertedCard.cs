using System.Collections;
using UnityEngine;
namespace PimPamPum
{
    public class ConvertedCard : Card
    {
        private Card original, converted;

        public override Card Original => original;
        public override Suit Suit => Original.Suit;
        public override Rank Rank => Original.Rank;
        public override Color Color => converted.Color;

        public ConvertedCard(Card original, Card converted)
        {
            this.original = original;
            this.converted = converted;
            Struct = original.Struct;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            converted.BeginCardDrag(pc);
        }

        public override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return converted.CardEffect(pc, player, drop, cardIndex);
        }

        public override IEnumerator CardUsed(PlayerController pc)
        {
            pc.UsedSkillCard();
            yield return original.CardUsed(pc);
        }

        public override bool Is<T>()
        {
            if (typeof(T) == typeof(ConvertedCard)) return true;
            return converted.Is<T>();
        }

        public override string ToString()
        {
            return original + " as " + converted;
        }
    }
}