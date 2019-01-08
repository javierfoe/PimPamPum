
namespace Bang
{
    public class TucoFranziskaner : PlayerController
    {

        protected override void DrawPhase1()
        {
            base.DrawPhase1();
            if(!HasProperties && HasColt45)
            {
                Draw(2);
            }
        }

        protected override string Character()
        {
            return "TucoFranziskaner";
        }

    }
}
