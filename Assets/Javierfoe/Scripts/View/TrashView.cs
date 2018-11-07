namespace Bang
{
    public class TrashView : DropView
    {
        protected override void Start()
        {
            base.Start();
            Droppable = true;
        }
    }
}