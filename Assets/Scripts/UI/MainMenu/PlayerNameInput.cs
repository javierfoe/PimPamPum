
namespace PimPamPum
{
    public class PlayerNameInput : InputField
    {
        private const string nameKey = "PlayerName";

        protected override void Awake()
        {
            key = nameKey;
            base.Awake();
        }
    }
}