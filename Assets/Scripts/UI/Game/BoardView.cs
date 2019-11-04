namespace PimPamPum
{
    public class BoardView : DropView
    {
        protected override void Awake()
        {
            base.Awake();
            drop = Drop.Board;
        }

        public override void SetTargetable(bool value)
        {
            Droppable = value;
            base.SetTargetable(value);
        }
    }
}