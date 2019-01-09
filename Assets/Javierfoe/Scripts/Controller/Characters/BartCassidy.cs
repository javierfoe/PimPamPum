
namespace Bang
{
    public class BartCassidy : PlayerController
    {

        protected override void HitTrigger(int attacker)
        {
            Draw();
        }

        protected override string Character()
        {
            return "Bart Cassidy";
        }

    }
}