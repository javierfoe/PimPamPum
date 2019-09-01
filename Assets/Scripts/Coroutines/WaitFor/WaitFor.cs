namespace PimPamPum
{
    public abstract class WaitFor : Enumerator
    {
        public static WaitFor CurrentWaitFor;

        protected WaitFor()
        {
            CurrentWaitFor = this;
        }

        public virtual void MakeDecision(int card) { }
        public virtual void MakeDecision(Decision decision, Card card = null) { }
        public virtual void MakeDecision(Decision phaseOneOption, int player, Drop dropEnum, int card) { }
    }
}