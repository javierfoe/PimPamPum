
namespace Bang
{
    public class SuzyLafayette : PlayerController
    {

        protected override void NoCardTrigger()
        {
            Draw(1);
        }

        protected override string Character()
        {
            return "SuzyLafayette";
        }

    }
}

