using System.Collections;
namespace PimPamPum
{
    public class Stagecoach : Draw
    {
        public Stagecoach() : base(2) { }

        public override IEnumerator CardUsed(PlayerController pc)
        {
            yield return GameController.Instance.UsedCard<Stagecoach>(pc);
        }

        public override string ToString()
        {
            return "Stagecoach";
        }
    }
}