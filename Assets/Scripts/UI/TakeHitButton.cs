using UnityEngine.UI;

namespace Bang
{

    public class TakeHitButton : Button
    {

        public void SetText(string text)
        {
            GetComponentInChildren<Text>().text = text;
        }

        protected override void Click()
        {
            PlayerController.LocalPlayer.TakeHit();
        }

    }
}
