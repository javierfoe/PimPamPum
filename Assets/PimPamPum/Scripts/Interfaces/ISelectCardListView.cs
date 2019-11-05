namespace PimPamPum
{
    public interface ISelectCardListView : ICardHolderView
    {
        void Enable(bool value);
        void EnableCards(bool value);
        void RemoveAllCards();
    }
}
