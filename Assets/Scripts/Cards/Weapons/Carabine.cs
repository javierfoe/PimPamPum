using System.Collections;

namespace PimPamPum
{
    public class Carabine : Weapon
    {
        public Carabine() : base(4) { }

        protected override IEnumerator EquipTrigger(PlayerController pc)
        {
            yield return pc.Equip<Carabine>(this);
        }

        public override string ToString()
        {
            return "Carabine";
        }
    }
}