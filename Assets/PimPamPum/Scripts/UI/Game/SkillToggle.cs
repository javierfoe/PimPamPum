using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PimPamPum
{
    public class SkillToggle : MonoBehaviour, ISkillView, IPointerClickHandler
    {
        private Image image;
        private Toggle toggle;

        private void Awake()
        {
            image = GetComponent<Image>();
            toggle = GetComponent<Toggle>();
        }

        public void SetActive(bool active)
        {
            if (!active) SetStatus(false);
            image.enabled = active;
            enabled = active;
        }

        public void SetStatus(bool enabled)
        {
            toggle.isOn = enabled;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PlayerController.LocalPlayer.UseSkillClient();
        }
    }
}