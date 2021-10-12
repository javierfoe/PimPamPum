namespace PimPamPum
{
    public abstract class ResponseCoroutine : Enumerator
    {
        protected int player;
        protected Decision currentDecision;

        public bool TakeHit { get; protected set; }

        public override abstract bool MoveNext();

        protected void SetCardResponse(int player, WaitForCardResponse timer)
        {
            Current = GameController.CardResponse(player, timer.ResponseCard);
        }

        public ResponseCoroutine() { }

        public ResponseCoroutine(PlayerController playerController)
        {
            SetPlayerController(playerController);
        }

        public virtual void SetPlayerController(PlayerController playerController)
        {
            player = playerController.PlayerNumber;
        }
    }
}
