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
        }

        public override void Click()
        {
            PlayerController.LocalPlayer.PhaseOneOption(PhaseOneOption.Player, IPlayerView.PlayerIndex, DropIndex);
        }
    }
}