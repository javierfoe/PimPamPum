namespace PimPamPum
{
    public abstract class WaitFor : Enumerator
    {

        public static WaitFor CurrentTimer;

        protected WaitFor()
        {
            CurrentTimer = this;
        }

        public virtual void MakeDecision(int card) { }
        public virtual void MakeDecision(Decision decision, Card card = null) { }

    }
}