namespace Bang
{
    public class TrashView : DropView
    {
        protected override void Awake()
        {
            base.Awake();
            Droppable = true;
            drop = Drop.Trash;
        }
    }
}