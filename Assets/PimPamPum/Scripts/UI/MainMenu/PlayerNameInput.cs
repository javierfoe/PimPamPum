
namespace PimPamPum
{
    public class PlayerNameInput : InputField
    {
        private const string nameKey = "PlayerName";

        protected override void Start()
        {
            key = nameKey;
            base.Start();
        }
    }
}