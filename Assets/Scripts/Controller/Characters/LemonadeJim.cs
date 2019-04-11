using System.Collections;

namespace PimPamPum
{

    public class LemonadeJim : PlayerController
    {

        protected override IEnumerator UsedBeerTrigger(int player)
        {
            if (player != PlayerNumber)
            {
                State previousState = State;
                State = State.SpecialEvent;
                EnablePassButton(true);
                EnableAllCards();
                yield return GameController.Instance.LemonadeJimBeerUsed(PlayerNumber);
                DisableCards();
                EnablePassButton(false);
                if (previousState == State.Play)
                {
                    Phase2();
                }
            }
        }

        protected override void UseCardState(int index, int player, Drop drop, int cardIndex)
        {
            if (State == State.SpecialEvent)
            {
                if(drop == Drop.Trash)
                {
                    DiscardCardFromHand(index);
                    Heal();
                    MakeDecision(Decision.Heal);
                }
            }
            else
            {
                base.UseCardState(index, player, drop, cardIndex);
            }
        }

    }

}