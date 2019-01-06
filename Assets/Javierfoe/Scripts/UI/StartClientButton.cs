using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class StartClientButton : NetworkManagerButton
{
    [SerializeField] private InputField hostAddress = null;

    private bool hostAddressError;
    private string errorAddress;

    private bool IsValidAddress
    {
        get
        {
            string text = hostAddress.text;
            return !string.IsNullOrEmpty(text) && Regex.IsMatch(text, "localhost|[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0.9]{1,3}");
        }
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(InputEmptyHostAddress());
    }

    protected override void NetworkManagerAction()
    {
        if (!hostAddressError && !IsValidAddress || hostAddressError)
        {
            if (!hostAddressError) hostAddressError = true;
            return;
        }
        networkManager.StartClient();
    }

    private void ErrorEnable(bool value)
    {
        hostAddress.interactable = !value;
        hostAddress.text = value ? "Invalid address" : errorAddress;
    }

    private IEnumerator InputEmptyHostAddress()
    {
        while (true)
        {
            if (hostAddressError)
            {
                errorAddress = hostAddress.text;
                ErrorEnable(true);
                yield return new WaitForSeconds(errorTime);
                ErrorEnable(false);
                hostAddressError = false;
            }
            yield return null;
        }
    }
}
