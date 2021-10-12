using System.Collections;

namespace PimPamPum
{
    public class WellsFargo : Draw
    {
        public WellsFargo() : base(3) { }

        public override IEnumerator CardUsed(PlayerController pc)
        {
            yield return GameController.UsedCard<WellsFargo>(pc);
        }

        public override string ToString()
        {
            return "Wells Fargo";
        }
    }
}