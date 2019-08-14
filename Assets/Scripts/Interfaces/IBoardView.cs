namespace PimPamPum
{
    public interface IBoardView : IDropView
    {
        void SetDeckSize(int cards);
        void SetDiscardTop(CardStruct cs);
        void EmptyDiscardStack();
    }
}