using System.Collections;
using UnityEngine;

namespace Bang
{
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public abstract class NetworkManagerButton : MonoBehaviour
    {
        protected static UnityEngine.UI.InputField playerName;
        protected static NetworkManager networkManager;

        public static string PlayerName
        {
            get
            {
                return playerName.text;
            }
        }

        protected const float errorTime = 0.5f;

        private bool playerNameError;

        // Use this for initialization
        protected virtual void Start()
        {
            InitializeStaticVariables();
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(this.Click);
            StartCoroutine(InputEmpty());
        }

        private void InitializeStaticVariables()
        {
            if (!networkManager) networkManager = FindObjectOfType<NetworkManager>();
            if (!playerName) playerName = FindObjectOfType<PlayerNameInput>().Input;
        }

        private void Click()
        {
            if (!playerNameError && string.IsNullOrEmpty(playerName.text) || playerNameError)
            {
                if (!playerNameError) playerNameError = true;
                return;
            }
            NetworkManagerAction();
        }

        private IEnumerator InputEmpty()
        {
            while (true)
            {
                if (playerNameError)
                {
                    ErrorEnable(true);
                    yield return new WaitForSeconds(errorTime);
                    ErrorEnable(false);
                    playerNameError = false;
                }
                yield return null;
            }
        }

        private void ErrorEnable(bool value)
        {
            playerName.interactable = !value;
            playerName.text = value ? "Type your name" : "";
        }

        protected abstract void NetworkManagerAction();

    }
}