using UnityEngine;
using UnityEngine.UI;

namespace Bang
{
    public abstract class DropView : MonoBehaviour, IDropView
    {

        [SerializeField] private Color highlight = new Color();

        protected int drop;
        private Image background;
        private Color idle;

        public bool Droppable
        {
            get; protected set;
        }

        protected virtual void Start()
        {
            background = GetComponent<Image>();
            idle = background.color;
            Droppable = false;
        }

        public GameObject GameObject()
        {
            return gameObject;
        }

        public bool GetDroppable()
        {
            return Droppable;
        }

        public void SetDroppable(bool value)
        {
            Droppable = value;
        }

        public void Highlight(bool value)
        {
            background.color = value ? highlight : idle;
        }

        public virtual int GetDropEnum()
        {
            return drop;
        }
    }
}