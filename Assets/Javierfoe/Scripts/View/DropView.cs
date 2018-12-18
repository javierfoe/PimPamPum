using UnityEngine;
using UnityEngine.UI;

namespace Bang
{
    public abstract class DropView : MonoBehaviour, IDropView
    {

        [SerializeField] protected Color highlight = new Color();

        protected Drop drop;
        protected Image background;
        protected Color idle;

        public bool Droppable
        {
            get; protected set;
        }

        protected virtual void Awake()
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

        public virtual void Highlight(bool value)
        {
            background.color = value ? highlight : idle;
        }

        public Drop GetDropEnum()
        {
            return drop;
        }

        public virtual int GetDropIndex()
        {
            return -1;
        }
    }
}