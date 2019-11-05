namespace PimPamPum
{
    public class Colt45 : Weapon
    {
        public Colt45() : base(1) { SetSuitRank(); }

        public override string ToString()
        {
            return "Colt45";
        }
    }
}