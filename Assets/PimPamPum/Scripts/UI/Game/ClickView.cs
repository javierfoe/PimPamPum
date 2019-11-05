using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace PimPamPum
{
    public abstract class ClickView : MonoBehaviour, IClickView
    {
        private bool clickable;

        public virtual void EnableClick(bool value)
        {
            clickable = value;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!clickable) return;
            Click();
        }

        public virtual void Click() { throw new NotImplementedException(); }
    }
}