
namespace Bang
{
    public class ElGringo : PlayerController
    {

        protected override void HitTrigger(int attacker)
        {
            if (attacker == PlayerNumber || attacker == BangConstants.NoOne) return;
            GameController.StealIfHandNotEmpty(PlayerNumber, attacker);
        }

        protected override string Character()
        {
            return "El Gringo";
        }

    }
}
