
public class HostAdressInputField : Bang.InputField {

    private const string hostAddressKey = "HostAddress";

    protected override void Awake()
    {
        key = hostAddressKey;
        base.Awake();
    }
}
