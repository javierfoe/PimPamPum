using Mirror;

namespace PimPamPum
{
    public class CountdownController : NetworkBehaviour
    {

        public ICountdownView CountdownView { get; set; }
        [field: SyncVar(hook = nameof(UpdateTimeSpent))] public float TimeSpent { get; set; }
        [field: SyncVar(hook = nameof(UpdateMaxTime))] public float MaxTime { get; set; }
        [field: SyncVar(hook = nameof(EnableCountdown))] public bool Enable { get; set; }

        private void UpdateTimeSpent(float timeSpent)
        {
            CountdownView?.SetTimeSpent(timeSpent);
        }

        private void UpdateMaxTime(float maxTime)
        {
            CountdownView?.SetCountdown(maxTime);
        }

        private void EnableCountdown(bool enable)
        {
            CountdownView?.Enable(enable);
        }

        public void UpdateOnHost()
        {
            UpdateTimeSpent(TimeSpent);
            UpdateMaxTime(MaxTime);
            EnableCountdown(Enable);
        }
    }
}