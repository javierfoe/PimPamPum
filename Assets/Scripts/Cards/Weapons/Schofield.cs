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

        public override string ToString()
        {
            return "Schofield";
        }
    }
}