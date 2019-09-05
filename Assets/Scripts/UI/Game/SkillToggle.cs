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
            image.enabled = active;
            enabled = active;
            if (!active) SetStatus(false);
        }

        public void SetStatus(bool enabled)
        {
            toggle.isOn = enabled;
            toggle.interactable = enabled;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PlayerController.LocalPlayer.UseSkillClient();
        }
    }
}