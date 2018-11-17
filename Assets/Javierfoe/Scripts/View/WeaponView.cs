namespace Bang
{
    public class WeaponView : CardView
    {
        protected override void Start()
        {
            base.Start();
            eDrop = EDrop.WEAPON;
        }

        public override int GetDropEnum()
        {
            return (int)eDrop;
        }
    }
}
