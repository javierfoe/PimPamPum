using UnityEngine;
using UnityEngine.UI;

namespace PimPamPum
{
    public abstract class DropView : ClickView, IDropView
    {
        [SerializeField] protected Color highlight = new Color(), target = new Color(), click = new Color();

        protected Image background;

        protected Drop drop;
        protected Color idle;
        private bool droppable;

        public virtual int PlayerNumber => IPlayerView != null ? IPlayerView.PlayerIndex : -1;
        public virtual int DropIndex => -1;
        public Drop DropEnum => drop;

        public IPlayerView IPlayerView
        {
            get; set;
        }

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

        protected void GetIPlayerViewInParent()
        {
            Transform transform = this.transform;
            do
            {
                IPlayerView = transform.gameObject.GetComponent<IPlayerView>();
                transform = transform.parent;
            } while (IPlayerView == null);
        }

        public void SetDroppable(bool value)
        {
            Droppable = value;
        }

        public void Highlight(bool value)
        {
            IPlayerView?.Highlight(value);
            SetBackgroundColor(value);
        }
    }
}