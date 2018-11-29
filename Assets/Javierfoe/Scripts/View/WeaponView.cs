namespace Bang
{
    public class WeaponView : CardView
    {
        protected override void Start()
        {
            base.Start();
            drop = Weapon;
        }

        public override int GetDropEnum()
        {
            return drop;
        }
    }
}
