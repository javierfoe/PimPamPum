
namespace Bang
{
    public class BigSpencer : PlayerController
    {

        protected override void DrawInitialCards()
        {
            Draw(5);
        }

        protected override void EnableBangReaction() { }

        protected override string Character()
        {
            return "Big Spencer";
        }

    }
}