
namespace PimPamPum
{
    public class HostAdressInputField : InputField
    {

        private const string hostAddressKey = "HostAddress";

        protected override void Start()
        {
            key = hostAddressKey;
            base.Start();
        }
    }
}