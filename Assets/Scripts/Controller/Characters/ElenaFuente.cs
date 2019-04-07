
namespace PimPamPum
{

    public class ElenaFuente : PlayerCardConverter
    {
        protected override void EnablePimPamPumReaction()
        {
            ConvertHandTo<Missed>();
            base.EnablePimPamPumReaction();
        }

        protected override string Character()
        {
            return "Elena Fuente";
        }
    }
}