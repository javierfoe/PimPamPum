using UnityEngine;
using UnityEngine.UI;

namespace PimPamPum
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

        protected override void Awake()
        {
            base.Awake();
            text = GetComponentInChildren<Text>();
            drop = Drop.Hand;
            Transform transform = this.transform;
            do
            {
                IPlayerView = transform.gameObject.GetComponent<IPlayerView>();
                transform = transform.parent;
            } while (IPlayerView == null);
        }

        public override void Click()
        {
            PlayerController.LocalPlayer.PhaseOneDecision(Decision.Player, IPlayerView.PlayerIndex, DropIndex);
        }
    }
}