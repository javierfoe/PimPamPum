
namespace Bang
{

    public class ElenaFuente : PlayerCardConverter
    {
        protected override void EnableBangReaction()
        {
            ConvertHandTo<Missed>();
            base.EnableBangReaction();
        }

        protected override string Character()
        {
            return "Elena Fuente";
        }
    }
}