using UnityEngine;
using UnityEngine.UI;

namespace PimPamPum
{
    public class PlayerAmountText : MonoBehaviour
    {

        private Text text;

        // Use this for initialization
        void Awake()
        {
            text = GetComponent<Text>();
        }

        public void SetText(float value)
        {
            text.text = ((int)value).ToString();
        }
    }
}