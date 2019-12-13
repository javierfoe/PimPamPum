namespace PimPamPum
{
    public interface ICountdownView
    {
        void Enable(bool value);
        void SetCountdown(float time);
        void SetTimeSpent(float time);
    }
}
