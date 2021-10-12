using System.Collections;
namespace PimPamPum
{
    public class Schofield : Weapon
    {
        public Schofield() : base(2) { }

        protected override IEnumerator EquipTrigger(PlayerController pc)
        {
            yield return pc.Equip<Schofield>(this);
        }

        public override IEnumerator CardUsed(PlayerController pc)
        {
            yield return GameController.UsedCard<Schofield>(pc);
        }

        public override string ToString()
        {
            return "Schofield";
        }
    }
}