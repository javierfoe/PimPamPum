
namespace PimPamPum
{
    public class SuzyLafayette : PlayerController
    {

        protected override void NoCardTrigger()
        {
            Draw();
        }

        protected override string Character()
        {
            return "Suzy Lafayette";
        }

    }
}

