
namespace PimPamPum
{

    public class IndianCoroutine : ResponseCoroutine
    {

        public override bool MoveNext()
        {
            if (FirstTime) return true;
            ResponseTimer responseTimer = Current as ResponseTimer;
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
            Current = new ResponseTimer();
        }

    }
}