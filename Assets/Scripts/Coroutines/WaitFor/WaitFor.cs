using UnityEngine;

namespace PimPamPum
{
    public class WaitFor : Enumerator
    {
        protected static WaitFor turnCorutine, mainCorutine, dyingCorutine;

        private float time, maxTime;

        public bool TimeUp { get; private set; }

        public static WaitFor StartTurnCorutine()
        {
            WaitFor turn = new WaitFor(GameController.TurnTime);
            turnCorutine = turn;
            return turn;
        }

        public static void MakeDecision(int card)
        {
            mainCorutine.MakeDecisionCardIndex(card);
        }

        public static void MakeDecision(Decision decision, Card card = null)
        {
            WaitFor waitFor = mainCorutine;
            if (decision == Decision.Die) waitFor = dyingCorutine;
            waitFor.MakeDecisionCard(decision, card);
        }

        public static void MakeDecision(Decision phaseOne, int player, Drop dropEnum, int card)
        {
            mainCorutine.MakeDecisionPhaseOne(phaseOne, player, dropEnum, card);
        }

        public override bool MoveNext()
        {
            time += Time.deltaTime;
            bool timer = time < maxTime;
            TimeUp = !timer;
            return timer;
        }

        protected WaitFor() : this(GameController.ReactionTime) { }

        protected WaitFor(float maxTime)
        {
            if (turnCorutine != null && !(turnCorutine.Current is WaitForDying))
            {
                turnCorutine.Current = this;
            }
            this.maxTime = maxTime;
            mainCorutine = this;
            time = 0;
        }

        public void StopCorutine()
        {
            time = maxTime;
        }

        public virtual void MakeDecisionCardIndex(int card) { }
        public virtual void MakeDecisionCard(Decision decision, Card card) { }
        public virtual void MakeDecisionPhaseOne(Decision phaseOneOption, int player, Drop dropEnum, int card) { }
    }
}