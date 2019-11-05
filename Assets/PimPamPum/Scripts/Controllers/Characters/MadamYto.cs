using System.Collections;
namespace PimPamPum
{
    public class MadamYto : PlayerController
    {
        public override IEnumerator UsedCard<T>(int player)
        {
            if (typeof(T) == typeof(Beer))
            {
                yield return PimPamPumEvent(this + " draws a card for the beer just used.");
                Draw(1);
            }
        }
    }
}

