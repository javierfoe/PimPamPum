using System.Collections;
namespace PimPamPum
{
    public class Volcanic : Weapon
    {
        public Volcanic() : base(1) { }

        public override bool PimPamPum() { return true; }

        protected override IEnumerator EquipTrigger(PlayerController pc)
        {
            yield return pc.Equip<Volcanic>(this);
        }

        public override IEnumerator CardUsed(PlayerController pc)
        {
            yield return GameController.Instance.UsedCard<Volcanic>(pc);
        }

        public override string ToString()
        {
            return "Volcanic";
        }
    }
}