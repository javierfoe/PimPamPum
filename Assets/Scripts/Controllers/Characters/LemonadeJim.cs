using System.Collections;

namespace PimPamPum
{
    public class LemonadeJim : PlayerController
    {
        public override IEnumerator UsedCard<T>(int player)
        {
            if (typeof(T) == typeof(Beer) && player != PlayerNumber)
            {
                State previousState = State;
                State = State.SpecialEvent;
                EnablePassButton(true);
                EnableAllCards();
                WaitForDecision timer = new WaitForDecision();
                yield return timer;
                if (timer.Decision == Decision.Heal)
                {
                    yield return PimPamPumEvent(this + " has used his special ability and healed 1 HP.");
                }
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