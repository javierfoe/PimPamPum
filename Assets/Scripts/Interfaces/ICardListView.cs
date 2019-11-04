namespace PimPamPum
{
    public interface ICardListView : ICardHolderView
    {
        void SetDroppable(bool value);
        void SetClickable(bool value);
        void SetPlayable(int index, bool value);
    }
}