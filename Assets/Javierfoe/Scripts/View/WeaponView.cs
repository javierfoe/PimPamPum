namespace Bang
{
    public class WeaponView : CardView
    {
        protected override void Awake()
        {
            base.Awake();
            drop = Drop.Weapon;
        }
    }
}
