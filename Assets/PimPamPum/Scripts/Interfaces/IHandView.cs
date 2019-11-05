namespace PimPamPum
{
    public interface IHandView : IDropView
    {
        string Text { set; }
        void SetActive(bool value);
    }
}