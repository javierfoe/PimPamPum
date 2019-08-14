
namespace PimPamPum
{
    public abstract class ResponseCoroutine : FirstTimeEnumerator
    {
        protected int player;
        protected Decision currentDecision;

        public bool TakeHit { get; protected set; }

        public override abstract bool MoveNext();

        protected void SetCardResponse(int player, ResponseTimer timer)
        {
            Current = GameController.Instance.CardResponse(player, timer.ResponseCard);
        }

        public ResponseCoroutine() { }

        public ResponseCoroutine(PlayerController playerController)
        {
            SetPlayerInt(playerController);
        }

        public virtual void SetPlayerController(PlayerController playerController)
        {
            SetPlayerInt(playerController);
        }

        private void SetPlayerInt(PlayerController playerController)
        {
            player = playerController.PlayerNumber;
        }

    }
}
