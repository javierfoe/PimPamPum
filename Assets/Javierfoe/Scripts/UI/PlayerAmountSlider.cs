using UnityEngine;
using UnityEngine.UI;

public class PlayerAmountSlider : MonoBehaviour {

    private void Start()
    {
        Slider slider = GetComponent<Slider>();
        slider.onValueChanged.Invoke(slider.value);
    }
}
