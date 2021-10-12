using UnityEngine;

namespace PimPamPum
{
    public class WaitFor : Enumerator
    {
        private bool response;

        public PlayerController PlayerController { get; private set; }
        public bool Response { get { return response; } set { response = value;} }
        public bool TimeUp { get; private set; }

        public static WaitFor StartTurnCorutine(PlayerController player)
        {
            WaitFor turn = new WaitFor(player, GameController.TurnTime, true);
            WaitForController.TurnCorutine = turn;
            return turn;
        }

        public override bool MoveNext()
        {
            Finished(false);
            return true;
        }

        protected void Finished(bool value)
        {
            if (finished || value) return;
            Finished();
            finished = true;
        }

        protected virtual void Finished() { }

        protected WaitFor(PlayerController player) : this(player, GameController.ReactionTime) { }

        protected WaitFor(PlayerController player, float maxTime, bool turn = false)
        {
            PlayerController = player;
            if (!turn) WaitForController.MainCorutine = this;
        }

        public void StopCorutine()
        {
            Finished(false);
        }

        public virtual void MakeDecisionCardIndex(int card) { }
        public virtual void MakeDecisionCard(Decision decision, Card card) { }
        public virtual void MakeDecisionPhaseOne(Decision phaseOneOption, int player, Drop dropEnum, int card) { }
    }
}
