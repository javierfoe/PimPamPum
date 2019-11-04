
namespace PimPamPum
{

    public class ElenaFuente : PlayerController
    {

        protected override void EnablePimPamPumReaction()
        {
            ConvertHandTo<Missed>();
            base.EnablePimPamPumReaction();
        }

    }
}