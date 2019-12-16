using System.Collections;

namespace PimPamPum
{
    public class JoseDelgado : PlayerController
    {
        private const int maximumPropertiesDiscarded = 2;

        private int propertiesDiscarded;

        protected override IEnumerator OnStartTurn()
        {
            propertiesDiscarded = 0;
            yield return base.OnStartTurn();
        }

        public override void BeginCardDrag(Card c)
        {
            base.BeginCardDrag(c);
            if (c.Is<Property>() && propertiesDiscarded < maximumPropertiesDiscarded)
            {
                Actions.Thrash = true;
            }
        }

        protected override void UseCardState(int index, int player, Drop drop, int cardIndex)
        {
            if(State == State.Play && drop == Drop.Board)
            {
                DiscardCardFromHand(index);
                Draw(2);
                propertiesDiscarded++;
                return;
            }
            base.UseCardState(index, player, drop, cardIndex);
        }
    }
}