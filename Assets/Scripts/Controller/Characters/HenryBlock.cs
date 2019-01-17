using System.Collections;

namespace Bang
{
    public class HenryBlock : PlayerController
    {

        public override IEnumerator StolenBy(int thief)
        {
            if (thief != PlayerNumber)
            {
                yield return GameController.Bang(BangConstants.NoOne, thief);
            }
            yield return base.StolenBy(thief);
        }

        protected override string Character()
        {
            return "Henry Block";
        }

    }
}
