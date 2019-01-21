using System.Collections;

namespace Bang
{
    public class MickDefender : PlayerController
    {

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
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
            AvoidButton();
            if (player != target)
                yield return GameController.AvoidCard(player, target);
            TakeHitButton();
        }

        protected override string Character()
        {
            return "Mick Defender";
        }

    }
}