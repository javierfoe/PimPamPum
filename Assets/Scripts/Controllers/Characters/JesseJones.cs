using System.Collections;

namespace PimPamPum
{
    public class JesseJones : PlayerController
    {
        protected override IEnumerator DrawPhase1()
        {
            if (GameController.Instance.SetPhaseOnePlayerHandsClickable(PlayerNumber))
            {
                WaitForPhaseOneChoice waitForPhaseOneChoice = new WaitForPhaseOneChoice(PlayerNumber);
                yield return waitForPhaseOneChoice;
                switch (waitForPhaseOneChoice.PhaseOneOption)
                {
                    case Decision.Deck:
                        yield return base.DrawPhase1();
                        break;
                    case Decision.Player:
                        GameController.Instance.StealCard(PlayerNumber, waitForPhaseOneChoice.Player);
                        Draw();                        
                        break;
                }
            }
            else
            {
                yield return base.DrawPhase1();
            }
        }
    }
}