using UnityEngine;
using UnityEngine.UI;

namespace PimPamPum
{
    public abstract class DropView : MonoBehaviour, IDropView
    {
        [SerializeField] protected Color highlight = new Color(), target = new Color(), clickable = new Color();

        private Image background;

        protected Drop drop;
        protected Color idle;
        private bool droppable;

        public virtual int GetDropIndex() => -1;
        public GameObject GameObject() => gameObject;
        public bool GetDroppable() => Droppable;
        public Drop GetDropEnum() => drop;

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

        public virtual void SetTargetable(bool value)
        {
            Targetable = value;
            SetBackgroundColor();
        }

        private void SetBackgroundColor(bool highlight = false)
        {
            Color color = GetColor(highlight);
            SetBackgroundColor(color);
        }

        protected void SetBackgroundColor(Color color)
        {
            background.color = color;
        }

        protected virtual Color GetColor(bool highlight)
        {
            return highlight ? this.highlight : Droppable || Targetable ? target : idle;
        }

        public void SetDroppable(bool value)
        {
            Droppable = value;
        }

        public void Highlight(bool value)
        {
            SetBackgroundColor(value);
        }
    }
}