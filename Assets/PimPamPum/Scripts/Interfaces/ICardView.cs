namespace PimPamPum
{
    public interface ICardView : IDropView, IClickView
    {
        void Playable(bool value);
        void Selectable(bool value);
        void SetIndex(int index);
        void SetCard(CardStruct cs);
        void Empty();
    }
}
