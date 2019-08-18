﻿using System.Collections;

namespace PimPamPum
{
    public class PimPamPum : Card
    {
        public override void BeginCardDrag(PlayerController pc)
        {
            base.BeginCardDrag(pc);
            pc.PimPamPumBeginCardDrag();
        }

        protected override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            yield return base.CardEffect(pc, player, drop, cardIndex);
            pc.CheckNoCards();
            yield return pc.ShotPimPamPum(player);
        }

        public override string ToString()
        {
            return "PimPamPum";
        }
    }
}