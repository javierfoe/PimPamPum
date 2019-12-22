using Mirror;

namespace PimPamPum
{
    public class CountdownController : NetworkBehaviour
    {
        public ICountdownView CountdownView { get; set; }
        [field: SyncVar(hook = nameof(UpdateTimeSpent))] public float TimeSpent { get; set; }

        private void UpdateTimeSpent(float timeSpent)
        {
            CountdownView?.SetTimeSpent(timeSpent);
        }

        public void UpdateOnHost()
        {
            UpdateTimeSpent(TimeSpent);
        }

        public void Disable()
        {
            RpcDisable();
        }

        public void SetCountdown(float time)
        {
            RpcSetCountdown(time);
        }

        [ClientRpc]
        private void RpcDisable()
        {
            CountdownView.Enable(false);
        }

        [ClientRpc]
        private void RpcSetCountdown(float time)
        {
            CountdownView.SetCountdown(time);
            CountdownView.Enable(true);
        }
    }
}