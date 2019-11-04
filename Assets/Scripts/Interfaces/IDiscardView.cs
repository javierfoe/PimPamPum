namespace PimPamPum
{
    public interface IDiscardView : IClickView
    {
        void SetDiscardTop(CardStruct cs);
        void EmptyDiscardStack();
    }
}