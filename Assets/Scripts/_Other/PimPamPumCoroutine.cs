
namespace PimPamPum
{

    public class PimPamPumCoroutine : FirstTimeEnumerator
    {
        private PlayerController playerController;
        private int misses, dodges, barrelsUsed, barrels, player;
        private bool dodge;
        private Decision currentDecision;

        public bool TakeHit { get; private set; }

        public override bool MoveNext()
        {
            if (FirstTime) return true;
            ResponseTimer responseTimer = Current as ResponseTimer;
            if (responseTimer != null)
            {
                switch (responseTimer.Decision)
                {
                    case Decision.Avoid:
                        currentDecision = Decision.Pending;
                        dodges++;
                        Current = GameController.Instance.CardResponse(player, responseTimer.ResponseCard);
                        break;
                    case Decision.Barrel:
                        currentDecision = Decision.Pending;
                        barrelsUsed++;
                        Current = new DrawEffectCoroutine(playerController, GameController.Instance.DecisionTime);
                        break;
                }
                return true;
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
                Current = new ResponseTimer(GameController.Instance.DecisionTime);
                return true;
            }
            TakeHit = dodges < misses || currentDecision == Decision.TakeHit;
            return false;
        }

        public PimPamPumCoroutine(PlayerController playerController, int misses = 1)
        {
            this.playerController = playerController;
            this.misses = misses;
            player = playerController.PlayerNumber;
            barrels = playerController.Barrels;
            barrelsUsed = 0;
            dodges = 0;
        }

    }
}