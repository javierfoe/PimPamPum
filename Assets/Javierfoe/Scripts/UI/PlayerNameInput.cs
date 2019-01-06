using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class PlayerNameInput : MonoBehaviour
{

    private const string nameKey = "PlayerName";

    public InputField InputField
    {
        get; private set;
    }

    // Use this for initialization
    void Awake()
    {
        InputField = GetComponent<InputField>();
        if (PlayerPrefs.HasKey(nameKey))
        {
            InputField.text = PlayerPrefs.GetString(nameKey);
        }
        InputField.onValueChanged.AddListener(NameChanged);
    }

    private void NameChanged(string name)
    {
        PlayerPrefs.SetString(nameKey, name);
    }
}
