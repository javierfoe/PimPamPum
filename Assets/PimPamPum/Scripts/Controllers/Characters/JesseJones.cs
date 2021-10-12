using System.Collections;

namespace PimPamPum
{
    public class JesseJones : PlayerController
    {
        protected override IEnumerator DrawPhase1()
        {
            if (GameController.SetPhaseOnePlayerHandsClickable(PlayerNumber))
            {
                WaitForClickChoice waitForPhaseOneChoice = new WaitForClickChoice(this);
                yield return waitForPhaseOneChoice;
                switch (waitForPhaseOneChoice.Decision)
                {
                    case Decision.Deck:
                        yield return base.DrawPhase1();
                        break;
                    case Decision.Player:
                        GameController.StealCard(PlayerNumber, waitForPhaseOneChoice.Player);
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