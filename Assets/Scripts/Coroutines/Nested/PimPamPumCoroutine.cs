
namespace PimPamPum
{

    public class PimPamPumCoroutine : ResponseCoroutine
    {
        private PlayerController playerController;
        private int misses, dodges, barrelsUsed, barrels;
        private bool dodge;

        public override bool MoveNext()
        {
            if (FirstTime) return true;
            WaitForResponse responseTimer = Current as WaitForResponse;
            if (responseTimer != null)
            {
                currentDecision = responseTimer.Decision;
                switch (responseTimer.Decision)
                {
                    case Decision.Avoid:
                        currentDecision = Decision.Pending;
                        dodges++;
                        SetCardResponse(player, responseTimer);
                        return true;
                    case Decision.Barrel:
                        currentDecision = Decision.Pending;
                        barrelsUsed++;
                        Current = new DrawEffectCoroutine(playerController);
                        return true;
                }
            }
            DrawEffectCoroutine drawEffectCoroutine = Current as DrawEffectCoroutine;
            if (drawEffectCoroutine != null)
            {
                Card drawEffectCard = drawEffectCoroutine.DrawEffectCard;
                dodge = Barrel.CheckCondition(drawEffectCard);
                dodges += dodge ? 1 : 0;
                Current = GameController.Instance.BarrelEffect(player, drawEffectCard, dodge);
                return true;
            }
            if (dodges < misses && currentDecision != Decision.TakeHit)
            {
                playerController.EnableMissedsResponse();
                if (barrelsUsed < barrels)
                {
                    playerController.EnableBarrelButton(true);
                }
                Current = new WaitForResponse();
                return true;
            }
            TakeHit = dodges < misses || currentDecision == Decision.TakeHit;
            return false;
        }

        public PimPamPumCoroutine() { }

        public PimPamPumCoroutine(PlayerController playerController, int misses = 1) : base(playerController)
        {
            this.misses = misses;
        }

        public override void SetPlayerController(PlayerController playerController)
        {
            base.SetPlayerController(playerController);
            this.playerController = playerController;
            misses = 1;
            barrels = playerController.Barrels;
            barrelsUsed = 0;
            dodges = 0;
        }

    }
}