
namespace Bang
{
    public class HostAdressInputField : InputField
    {

        private const string hostAddressKey = "HostAddress";

        protected override void Awake()
        {
            key = hostAddressKey;
            base.Awake();
        }
    }
}