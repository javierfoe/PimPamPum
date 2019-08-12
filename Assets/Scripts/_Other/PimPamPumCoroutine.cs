
namespace PimPamPum
{

    public class PimPamPumCoroutine : FirstTimeEnumerator
    {

        private int misses, dodges, barrelsUsed, barrel;
        private bool dodge;
        private Decision currentDecision;

        public Decision Decision { get; set; }

        public override bool MoveNext()
        {
            if (FirstTime) return true;
            bool res = dodges < misses && currentDecision != Decision.TakeHit;
            if (res)
            {
                Current = new ResponseTimer(0);
            }
            else
            {

            }
            return res;
        }

    }

}