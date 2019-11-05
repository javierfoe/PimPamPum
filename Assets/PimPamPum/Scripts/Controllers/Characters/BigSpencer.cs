
namespace PimPamPum
{
    public class BigSpencer : PlayerController
    {

        protected override void DrawInitialCards()
        {
            Draw(5);
        }

        protected override void EnablePimPamPumReaction() { }

    }
}