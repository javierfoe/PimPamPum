
namespace PimPamPum
{
    public class ElGringo : PlayerController
    {

        protected override void HitTrigger(int attacker)
        {
            if (attacker == PlayerNumber || attacker == PimPamPumConstants.NoOne) return;
            GameController.Instance.StealIfHandNotEmpty(PlayerNumber, attacker);
        }

        protected override string Character()
        {
            return "El Gringo";
        }

    }
}
