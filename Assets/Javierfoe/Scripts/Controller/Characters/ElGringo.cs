
namespace Bang
{
    public class ElGringo : PlayerController
    {

        protected override void HitTrigger(int attacker)
        {
            PlayerController attackerPc = GameController.GetPlayerController(attacker);
            Card c = null;
            if (attackerPc.HasCards)
            {
                c = attackerPc.StealCardFromHand();
                AddCard(c);
            }
        }

        protected override string Character()
        {
            return "El Gringo";
        }

    }
}
