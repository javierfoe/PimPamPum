using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PimPamPum
{
    [RequireComponent(typeof(Slider))]
    public class PlayerAmountSlider : MonoBehaviour
    {
        private static Slider slider;

        public static void AddListener(UnityAction<float> unityEvent)
        {
            slider.onValueChanged.AddListener(unityEvent);
            unityEvent.Invoke(slider.value);
        }

        private void Awake()
        {
            slider = GetComponent<Slider>();
        }
    }
}