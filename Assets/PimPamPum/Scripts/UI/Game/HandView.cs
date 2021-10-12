using UnityEngine;
using UnityEngine.UI;

namespace PimPamPum
{
    public class HandView : DropView, IHandView
    {
        [SerializeField] private Text text = null;

        public string Text
        {
            set {
                text.text = value;
            }
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        protected override void Awake()
        {
            base.Awake();
            drop = Drop.Hand;
            GetIPlayerViewInParent();
        }

        public override void EnableClick(bool value)
        {
            base.EnableClick(value);
            SetBackgroundColor(value ? Color.magenta : idle);
        }

        public override void Click()
        {
            PlayerController.CurrentPlayableCharacter.PhaseOneDecision(Decision.Player, IPlayerView.PlayerIndex);
        }
    }
}