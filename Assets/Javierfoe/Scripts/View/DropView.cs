using UnityEngine;
using UnityEngine.UI;

namespace Bang
{
    public abstract class DropView : MonoBehaviour, IDropView
    {

        [SerializeField] protected Color highlight = new Color(), target = new Color();

        protected Drop drop;
        protected Image background;
        protected Color idle;
        private bool droppable;

        private bool Targetable
        {
            get; set;
        }

        public bool Droppable
        {
            get { return droppable; }
            set
            {
                droppable = value;
                SetBackgroundColor();
            }
        }

        protected virtual void Awake()
        {
            background = GetComponent<Image>();
            idle = background.color;
            Droppable = false;
        }

        public void SetTargetable(bool value)
        {
            Targetable = value;
            SetBackgroundColor();
        }

        private void SetBackgroundColor(bool highlight = false)
        {
            background.color = GetColor(highlight);
        }

        protected virtual Color GetColor(bool highlight)
        {
            return highlight ? this.highlight : Droppable || Targetable ? target : idle;
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
            SetBackgroundColor(value);
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