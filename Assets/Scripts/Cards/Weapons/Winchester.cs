using System.Collections;

namespace PimPamPum
{
    public class Winchester : Weapon
    {
        public Winchester() : base(5) { }

        protected override IEnumerator EquipTrigger(PlayerController pc)
        {
            yield return pc.Equip<Winchester>(this);
        }

        public override string ToString()
        {
            return "Winchester";
        }
    }
}