
namespace Bang
{
    public class SuzyLafayette : PlayerController
    {

        public override void CheckNoCards()
        {
            Draw(1);
        }

        protected override string Character()
        {
            return "SuzyLafayette";
        }

    }
}

