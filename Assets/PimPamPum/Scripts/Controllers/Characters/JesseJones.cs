using System.Collections;

namespace PimPamPum
{
    public class JesseJones : PlayerController
    {
        protected override IEnumerator DrawPhase1()
        {
            if (GameController.Instance.SetPhaseOnePlayerHandsClickable(PlayerNumber))
            {
                WaitForClickChoice waitForPhaseOneChoice = new WaitForClickChoice(PlayerNumber);
                yield return waitForPhaseOneChoice;
                switch (waitForPhaseOneChoice.Decision)
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