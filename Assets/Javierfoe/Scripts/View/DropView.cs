using UnityEngine;
using UnityEngine.UI;

namespace Bang
{
    public class DropView : MonoBehaviour, IDropView
    {

        [SerializeField] private Color highlight = new Color();

        private Image background;
        private Color idle;
        private ECardDropArea dropArea;

        public bool Droppable
        {
            get; private set;
        }

        public ECardDropArea DropArea
        {
            get { return dropArea; }
            protected set
            {
                dropArea = value;
                Droppable = !(value < 0);
            }
        }

        protected virtual void Start()
        {
            background = GetComponent<Image>();
            idle = background.color;
            DropArea = ECardDropArea.NULL;
        }

        public GameObject GameObject()
        {
            return gameObject;
        }

        public void SetDroppable(ECardDropArea cda)
        {
            DropArea = cda;
        }

        public void Highlight(bool value)
        {
            background.color = value ? highlight : idle;
        }
    }
}