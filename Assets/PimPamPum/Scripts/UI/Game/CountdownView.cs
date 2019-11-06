using UnityEngine;
using UnityEngine.UI;
namespace PimPamPum
{
    public class CountdownView : MonoBehaviour, ICountdown
    {

        [System.Serializable]
        public class ColorThreshold
        {
            public Color color;
            public float value;
        }

        [SerializeField]
        private ColorThreshold[] colorThresholds;
        private Image image;
        private float maxTime;
        private int currentThreshold;

        private Color Color
        {
            set { image.color = value; }
        }

        private float FillAmount
        {
            set
            {
                image.fillAmount = value;
                SetCurrentColor(value);
            }
        }

        private void SetCurrentColor(float fillAmount)
        {
            if(fillAmount <= 0)
            {
                gameObject.SetActive(false);
                return;
            }
            if (fillAmount <= colorThresholds[currentThreshold].value)
            {
                currentThreshold++;
            }
            Color = colorThresholds[currentThreshold].color;
        }

        // Start is called before the first frame update
        void Awake()
        {
            image = GetComponent<Image>();
        }

        public void SetCountdown(float time)
        {
            maxTime = time;
            currentThreshold = 0;
            FillAmount = 1;
            gameObject.SetActive(true);
        }

        public void SetTimeSpent(float time)
        {
            FillAmount = 1 - (time / maxTime);
        }
    }
}