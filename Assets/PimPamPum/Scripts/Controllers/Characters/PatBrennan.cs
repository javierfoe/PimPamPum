using System.Collections;

namespace PimPamPum
{
    public class PatBrennan : PlayerController
    {
        protected override IEnumerator DrawPhase1()
        {
            if (GameController.Instance.SetPhaseOnePlayerPropertiesClickable(PlayerNumber))
            {
                WaitForClickChoice waitForPhaseOneChoice = new WaitForClickChoice(PlayerNumber);
                yield return waitForPhaseOneChoice;
                switch (waitForPhaseOneChoice.Decision)
                {
                    case Decision.Deck:
                        yield return base.DrawPhase1();
                        break;
                    case Decision.Player:
                        int targetPlayer = waitForPhaseOneChoice.Player;
                        switch (waitForPhaseOneChoice.Drop)
                        {
                            case Drop.Properties:
                                GameController.Instance.StealProperty(PlayerNumber, targetPlayer, waitForPhaseOneChoice.CardIndex);
                                break;
                            case Drop.Weapon:
                                GameController.Instance.StealWeapon(PlayerNumber, targetPlayer);
                                break;
                        }
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