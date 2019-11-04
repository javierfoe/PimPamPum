using UnityEngine;
using UnityEngine.UI;

namespace PimPamPum
{
    public class PlayerAmountText : MonoBehaviour
    {
        private Text text;

        // Use this for initialization
        void Start()
        {
            text = GetComponent<Text>();
            PlayerAmountSlider.AddListener(SetText);
        }

        public void SetText(float value)
        {
            text.text = ((int)value).ToString();
        }
    }
}