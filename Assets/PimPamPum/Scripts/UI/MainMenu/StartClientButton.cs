using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

namespace PimPamPum
{

    public class StartClientButton : NetworkManagerButton
    {
        private UnityEngine.UI.InputField hostAddress;

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
            hostAddress = FindObjectOfType<HostAdressInputField>().Input;
            hostAddress.onEndEdit.AddListener(UpdateNetworkAddress);
            UpdateNetworkAddress(hostAddress.text);

            StartCoroutine(InputEmptyHostAddress());
        }

        private void UpdateNetworkAddress(string address)
        {
            networkManager.networkAddress = address;
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
}