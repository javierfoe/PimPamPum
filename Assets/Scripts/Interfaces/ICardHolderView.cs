namespace PimPamPum
{
    public interface ICardHolderView
    {
        void AddCard(int index, CardStruct cs);
        void RemoveCard(int index);
    }
}