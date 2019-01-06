using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class NetworkManagerButton : MonoBehaviour
{
    protected static InputField playerName;
    protected static NetworkManagerScript networkManager;

    protected const float errorTime = 0.5f;

    private bool playerNameError;

    // Use this for initialization
    protected virtual void Start()
    {
        InitializeStaticVariables();
        networkManager = FindObjectOfType<NetworkManagerScript>();
        GetComponent<Button>().onClick.AddListener(Click);
        StartCoroutine(InputEmpty());
    }

    private void InitializeStaticVariables()
    {
        if (!networkManager) networkManager = FindObjectOfType<NetworkManagerScript>();
        if (!playerName) playerName = FindObjectOfType<PlayerNameInput>().InputField;
    }

    private void Click()
    {
        if (!playerNameError && string.IsNullOrEmpty(playerName.text) || playerNameError)
        {
            if(!playerNameError) playerNameError = true;
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
