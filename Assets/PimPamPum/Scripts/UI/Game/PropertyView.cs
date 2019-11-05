
namespace PimPamPum
{
    public class PropertyView : CardView
    {
        protected override void Awake()
        {
            base.Awake();
            drop = Drop.Properties;
        }
    }
}