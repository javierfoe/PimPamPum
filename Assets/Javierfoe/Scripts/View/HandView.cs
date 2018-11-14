using UnityEngine.UI;

namespace Bang
{
    public class HandView : DropView
    {
        private Text text;

        public string Text
        {
            set {
                text.text = value;
            }
        }

        protected override void Start()
        {
            base.Start();
            text = GetComponentInChildren<Text>();
            eDrop = EDrop.HAND;
        }
    }
}