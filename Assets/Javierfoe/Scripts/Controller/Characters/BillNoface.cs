
namespace Bang
{
    public class BillNoface : PlayerController
    {

        protected override void DrawPhase1()
        {
            Draw(1 + MaxHP - HP);
        }

        protected override string Character()
        {
            return "BillNoFace";
        }

    }
}
