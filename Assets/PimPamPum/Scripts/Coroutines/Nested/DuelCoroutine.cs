namespace PimPamPum
{
    public class DuelCoroutine : Enumerator
    {
        private PlayerController player, target, next;
        private int pimPamPumsTarget;
        private Decision decision;

        public override bool MoveNext()
        {
            if (finished) return false;
            WaitForCardResponse responseCoroutine = Current as WaitForCardResponse;
            if (responseCoroutine != null)
            {
                decision = responseCoroutine.Decision;
                if (decision == Decision.Avoid)
                {
                    Current = GameController.PimPamPumEvent(next + " keeps dueling.");
                    if (next == target)
                    {
                        pimPamPumsTarget++;
                    }
                    return true;
                }
                else
                {
                    Current = GameController.PimPamPumEvent(next + " loses the duel.");
                    return true;
                }
            }
            if(decision != Decision.TakeHit)
            {
                next = next == player ? target : player;
                next.EnablePimPamPumsDuelResponse();
                Current = new WaitForCardResponse(next);
                return true;
            }
            else
            {
                target.FinishResponse(pimPamPumsTarget);
                Current = GameController.HitPlayer(player.PlayerNumber, next);
                finished = true;
                return true;
            }
        }

        public DuelCoroutine(PlayerController player, PlayerController target)
        {
            this.player = player;
            this.target = target;
            next = player;
            decision = Decision.Pending;
        }
    }
}
