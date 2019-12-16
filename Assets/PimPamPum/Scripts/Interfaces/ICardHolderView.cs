namespace PimPamPum
{
    public interface ICardHolderView
    {
        void AddCard(int index, CardValues cs, IPlayerView iPlayerview = null);
        void RemoveCard(int index);
    }
}