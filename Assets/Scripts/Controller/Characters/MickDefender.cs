﻿using System.Collections;

namespace PimPamPum
{
    public class MickDefender : PlayerController
    {

        protected override void OnSetLocalPlayer()
        {
            PlayerView.SetTextTakeHitButton("Take Card Effect");
        }

        protected override void EnableIndiansReaction()
        {
            base.EnableIndiansReaction();
            for (int i = 0; i < hand.Count; i++)
            {
                if (hand[i] is Missed)
                {
                    TargetEnableCard(connectionToClient, i, true);
                }
            }
        }

        public override IEnumerator AvoidCard(int player, int target)
        {
            if (player != target)
            {
                AvoidButton();
                yield return GameController.Instance.AvoidCard(player, target);
                TakeHitButton();
            }
        }

        protected override string Character()
        {
            return "Mick Defender";
        }

    }
}