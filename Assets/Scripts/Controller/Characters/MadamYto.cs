using System.Collections;

namespace Bang
{
    public class MadamYto : PlayerController
    {

        public override IEnumerator UsedBeer()
        {
            if (!IsDead)
            {
                yield return BangEvent(this + " draws a card for the beer just used.");
                Draw(1);
            }
        }

        protected override string Character()
        {
            return "Madam Yto";
        }

    }
}

