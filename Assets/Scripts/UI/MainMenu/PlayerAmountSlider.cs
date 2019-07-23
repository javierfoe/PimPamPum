using UnityEngine;
using UnityEngine.UI;

namespace PimPamPum
{
    [RequireComponent(typeof(Slider))]
    public class PlayerAmountSlider : MonoBehaviour
    {

        private void Start()
        {
            Slider slider = GetComponent<Slider>();
            NetworkManager networkManager = FindObjectOfType<NetworkManager>();
            slider.onValueChanged.AddListener(networkManager.SetPlayerAmount);
            slider.onValueChanged.Invoke(slider.value);
        }
    }
}