namespace PimPamPum
{
    public abstract class Weapon : Property
    {
        public int Range
        {
            get; private set;
        }

        protected Weapon(int range)
        {
            Range = range;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            base.BeginCardDrag(pc);
            pc.SelfTargetCard();
        }

        public override void EquipProperty(PlayerController pc)
        {
            pc.EquipWeapon(this);
        }

        public virtual bool PimPamPum(PlayerController pc)
        {
            return pc.PimPamPum();
        }
    }
}