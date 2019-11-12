using System.Collections;

namespace PimPamPum
{
    public abstract class PlayerDiscardTwoCards : PlayerController
    {
        protected bool skillUsed = false;
        private int cardsDiscarded, firstCard, secondCard;
        private State previousState;

        public override void BeginCardDrag(Card c)
        {
            if (cardsDiscarded < 1 && State == State.Play && (!(c is PimPamPum) || CanShoot))
                base.BeginCardDrag(c);
            if (skillUsed) return;
            if (Hand.Count > 1 || cardsDiscarded > 0)
                GameController.Instance.HighlightTrash(PlayerNumber, true);
        }

        protected override void UseCardState(int index, int player, Drop drop, int cardIndex)
        {
            if (ValidState() && drop == Drop.Board)
            {
                cardsDiscarded = 1;
                previousState = State;
                State = State.SpecialEvent;
                firstCard = index;
                EnableCancelButton(true);
                EnableCards();
                StartCoroutine(StartDiscarding());
                return;
            }
            if (State == State.SpecialEvent && drop == Drop.Board)
            {
                cardsDiscarded = 0;
                secondCard = index;
                MakeDecisionServer(Decision.Confirm);
            }
            base.UseCardState(index, player, drop, cardIndex);
        }

        private IEnumerator StartDiscarding()
        {
            WaitForDecision waitForDecision = new WaitForDecision(this, Decision.Cancel);
            yield return waitForDecision;
            if (waitForDecision.TimeUp)
            {
                StopDiscarding();
            }
            else if (waitForDecision.Decision == Decision.Cancel)
            {
                StopDiscarding(true);
            }
            else if (waitForDecision.Decision == Decision.Confirm)
            {
                yield return SpecialActionEvent();
            }
        }

        private void StopDiscarding(bool cancel = false)
        {
            State = previousState;
            EnableCancelButton(false);
            EnableConfirmButton(false);
            if (previousState == State.Play)
            {
                EnableEndTurnButton(true);
                EnableCards();
            }
            if (cancel && previousState == State.Dying)
            {
                EnableDieButton(true);
                EnableCards();
            }
        }

        protected override void EnableCards(CardType card = CardType.PimPamPum)
        {
            if (!skillUsed)
            {
                if (ValidState())
                {
                    EnableAllCards();
                    return;
                }
                if (State == State.SpecialEvent)
                {
                    for (int i = 0; i < Hand.Count; i++)
                    {
                        TargetEnableCard(connectionToClient, i, i != firstCard);
                    }
                    return;
                }
            }
            base.EnableCards(card);
        }

        protected virtual bool ValidState()
        {
            return State == State.Play;
        }

        private IEnumerator SpecialActionEvent()
        {
            Card one = Hand[firstCard];
            Card two = Hand[secondCard];
            EnableConfirmOptions(one, two);
            WaitForClickChoice clickChoice = new WaitForClickChoice(this, Decision.Cancel);
            yield return clickChoice;
            EnableCancelButton(false);
            GameController.Instance.DisablePhaseOneClickable(PlayerNumber);
            if (clickChoice.Decision != Decision.Cancel)
            {
                DiscardCardFromHand(firstCard);
                if (firstCard < secondCard) secondCard--;
                DiscardCardFromHand(secondCard);
                yield return SpecialAction(clickChoice.Player);
                StopDiscarding();
            }
            else
            {
                StopDiscarding(!clickChoice.TimeUp);
            }
        }

        protected abstract void EnableConfirmOptions(Card one, Card two);
        protected abstract IEnumerator SpecialAction(int player);
    }
}