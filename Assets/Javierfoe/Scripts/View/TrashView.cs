namespace Bang
{
    public class TrashView : DropView
    {
        protected override void Start()
        {
            base.Start();
            DropArea = ECardDropArea.CANCEL;
        }
    }
}