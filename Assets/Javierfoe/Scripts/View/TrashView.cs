namespace Bang
{
    public class TrashView : DropView
    {
        public override int[] GetIndexes()
        {
            return new int[] { -1 };
        }

        protected override void Start()
        {
            base.Start();
            DropArea = ECardDropArea.CANCEL;
        }
    }
}