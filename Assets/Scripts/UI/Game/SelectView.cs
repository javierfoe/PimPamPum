using UnityEngine;
using System.Collections;

namespace PimPamPum
{
    public abstract class SelectView : DropView
    {
        private UnityEngine.UI.Button button;

        public virtual void EnableClick(bool value)
        {
            if (!button)
            {
                button = gameObject.AddComponent<UnityEngine.UI.Button>();
                button.onClick.AddListener(Click);
            }
            button.interactable = value;
        }

        protected abstract void Click();
    }
}