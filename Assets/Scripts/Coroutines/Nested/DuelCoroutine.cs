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
            WaitForResponse responseCoroutine = Current as WaitForResponse;
            if (responseCoroutine != null)
            {
                decision = responseCoroutine.Decision;
                if (decision == Decision.Avoid)
                {
                    Current = GameController.Instance.PimPamPumEvent(next + " keeps dueling.");
                    if (next == target)
                    {
                        pimPamPumsTarget++;
                    }
                    return true;
                }
                else
                {
                    Current = GameController.Instance.PimPamPumEvent(next + " loses the duel.");
                    return true;
                }
            }
            if(decision != Decision.TakeHit)
            {
                next = next == player ? target : player;
                next.EnablePimPamPumsDuelResponse();
                Current = new WaitForResponse();
                return true;
            }
            else
            {
                player.CheckNoCards();
                target.FinishResponse(pimPamPumsTarget);
                Current = GameController.Instance.HitPlayer(player.PlayerNumber, next);
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
