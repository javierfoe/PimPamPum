
namespace PimPamPum
{

    public class IndianCoroutine : ResponseCoroutine
    {

        private bool first = true;

        public override bool MoveNext()
        {
            if (first)
            {
                first = false;
                return true;
            }
            WaitForResponse responseTimer = Current as WaitForResponse;
            if (responseTimer != null)
            {
                currentDecision = responseTimer.Decision;
                switch (responseTimer.Decision)
                {
                    case Decision.Avoid:
                        SetCardResponse(player, responseTimer);
                        return true;
                }
            }
            TakeHit = currentDecision == Decision.TakeHit;
            return false;
        }

        public override void SetPlayerController(PlayerController playerController)
        {
            base.SetPlayerController(playerController);
            playerController.EnablePimPamPumsResponse();
            Current = new WaitForResponse();
        }

    }
}