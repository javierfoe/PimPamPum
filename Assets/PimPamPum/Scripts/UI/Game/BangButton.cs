using UnityEngine;

namespace PimPamPum
{

    public abstract class Button : MonoBehaviour
    {

        public bool Active
        {
            set
            {
                gameObject.SetActive(value);
            }
        }

        // Use this for initialization
        protected virtual void Start()
        {
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => Click());
        }

        protected abstract void Click();

    }
}
