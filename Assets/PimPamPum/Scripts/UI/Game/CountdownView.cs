using UnityEngine;
using UnityEngine.UI;
namespace PimPamPum
{
    public class CountdownView : MonoBehaviour, ICountdownView
    {

        [System.Serializable]
        public class ColorThreshold
        {
            public Color color;
            public float value;
        }

        [SerializeField] private ColorThreshold[] colorThresholds = null;
        [SerializeField] private Image image = null;

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
                if (value <= 0)
                {
                    Enable(false);
                    return;
                }
                if (value <= colorThresholds[currentThreshold].value)
                {
                    currentThreshold++;
                }
                Color = colorThresholds[currentThreshold].color;
            }
        }

        public void Enable(bool value)
        {
            gameObject.SetActive(value);
            currentThreshold = 0;
            if (value) FillAmount = 1;
        }

        public void SetCountdown(float time)
        {
            maxTime = time;
        }

        public void SetTimeSpent(float time)
        {
            FillAmount = 1 - (time / maxTime);
        }
    }
}