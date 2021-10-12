
namespace PimPamPum
{
    public class ElGringo : PlayerController
    {

        protected override void HitTrigger(int attacker)
        {
            if (attacker == PlayerNumber || attacker == PimPamPumConstants.NoOne) return;
            GameController.StealIfHandNotEmpty(PlayerNumber, attacker);
        }

    }
}
