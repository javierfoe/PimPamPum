namespace PimPamPum
{
    public interface ICardView : ISelectView
    {
        void Playable(bool value);
        void SetIndex(int index);
        void SetCard(CardStruct cs);
        void Empty();
    }
}
