using UnityEngine;

namespace PimPamPum
{
    public class WaitFor : Enumerator
    {
        private float time, maxTime;
        private bool response;
        private int frame;

        public PlayerController PlayerController { get; private set; }
        public bool Response { get { return response; } set { response = value; SetCountdown(); } }
        public bool TimeUp { get; private set; }

        public static WaitFor StartTurnCorutine(PlayerController player)
        {
            WaitFor turn = new WaitFor(player, GameController.TurnTime, true);
            WaitForController.TurnCorutine = turn;
            return turn;
        }

        public override bool MoveNext()
        {
            int currentFrame = Time.frameCount;
            if (frame != currentFrame)
            {
                frame = currentFrame;
                time += Time.deltaTime;
            }
            bool timer = time < maxTime;
            TimeUp = !timer;
            SetTimeSpent();
            Finished(timer);
            return timer;
        }

        protected void Finished(bool value)
        {
            if (finished || value) return;
            Finished();
            MaxTimeSpent();
            finished = true;
        }

        protected virtual void Finished() { }

        protected WaitFor(PlayerController player) : this(player, GameController.ReactionTime) { }

        protected WaitFor(PlayerController player, float maxTime, bool turn = false)
        {
            PlayerController = player;
            frame = -1;
            this.maxTime = maxTime;
            if (!turn) WaitForController.MainCorutine = this;
            time = 0;
        }

        public void StopCorutine()
        {
            time = maxTime;
            Finished(false);
        }

        public void SetCountdown()
        {
            if (Response)
            {
                PlayerController.ResponseEnable = true;
                PlayerController.ResponseMaxTime = maxTime;
            }
            else
            {
                PlayerController.TurnEnable = true;
                PlayerController.TurnMaxTime = maxTime;
            }
        }

        private void MaxTimeSpent()
        {
            time = maxTime;
            SetTimeSpent();
            if (Response)
            {
                PlayerController.ResponseEnable = false;
            }
            else
            {
                PlayerController.TurnEnable = false;
            }
        }

        public void SetTimeSpent()
        {
            if (Response)
            {
                PlayerController.ResponseTime = time;
            }
            else
            {
                PlayerController.TurnTime = time;
            }
        }

        public virtual void MakeDecisionCardIndex(int card) { }
        public virtual void MakeDecisionCard(Decision decision, Card card) { }
        public virtual void MakeDecisionPhaseOne(Decision phaseOneOption, int player, Drop dropEnum, int card) { }
    }
}
