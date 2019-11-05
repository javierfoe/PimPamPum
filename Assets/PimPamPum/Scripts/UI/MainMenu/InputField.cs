using UnityEngine;
using Input = UnityEngine.UI.InputField;

namespace PimPamPum
{
    [RequireComponent(typeof(Input))]
    public class InputField : MonoBehaviour
    {
        protected string key;

        public Input Input
        {
            get; private set;
        }

        // Use this for initialization
        void Awake()
        {
            Input = GetComponent<Input>();
        }

        protected virtual void Start()
        {
            if (PlayerPrefs.HasKey(key))
            {
                Input.text = PlayerPrefs.GetString(key);
            }
            Input.onEndEdit.AddListener(ValueChanged);
        }

        private void ValueChanged(string value)
        {
            PlayerPrefs.SetString(key, value);
        }
    }
}