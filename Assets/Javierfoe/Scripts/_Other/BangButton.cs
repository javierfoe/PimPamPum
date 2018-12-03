using UnityEngine;
using UnityEngine.UI;

namespace Bang
{

    public abstract class BangButton : MonoBehaviour
    {

        public bool Active
        {
            set
            {
                gameObject.SetActive(value);
            }
        }

        // Use this for initialization
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => Click());
        }

        protected abstract void Click();

    }
}
