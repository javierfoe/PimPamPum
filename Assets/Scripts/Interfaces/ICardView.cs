namespace PimPamPum
{
    public interface ICardView : IDropView
    {
        void Playable(bool value);
        void SetIndex(int index);
        void SetCard(CardStruct cs);
        void Empty();
    }
}
