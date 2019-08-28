namespace PimPamPum
{
    public interface ICardHolderView
    {
        void AddCard(int index, CardStruct cs, IPlayerView iPlayerview = null);
        void RemoveCard(int index);
    }
}