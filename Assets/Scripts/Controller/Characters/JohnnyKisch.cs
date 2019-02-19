using System.Collections;

namespace Bang
{
    public class JohnnyKisch : PlayerController
    {

        public override IEnumerator Equip<T>(Property p)
        {
            yield return GameController.Instance.DiscardCopiesOf<T>(PlayerNumber, p);
        }

        protected override string Character()
        {
            return "Johnny Kisch";
        }

    }
}