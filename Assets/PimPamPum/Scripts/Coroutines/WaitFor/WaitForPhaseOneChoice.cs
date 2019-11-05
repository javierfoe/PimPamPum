namespace PimPamPum
{
    public class WaitForClickChoice : WaitForDecision
    {
        private int player;

        public int Player
        {
            get; private set;
        }

        public Drop Drop
        {
            get; private set;
        }

        public int CardIndex
        {
            get; private set;
        }

        public override bool MoveNext()
        {
            bool res = base.MoveNext();
            if (!res) Finished();
            return res;
        }

        public void Finished()
        {
            GameController.Instance.DisablePhaseOneClickable(player);
        }

        public WaitForClickChoice(int player, Decision decision) : base(decision)
        {
            this.player = player;
        }

        public WaitForClickChoice(int player) : this(player, Decision.Deck) { }

        public override void MakeDecisionPhaseOne(Decision phaseOneOption, int player, Drop dropEnum, int card)
        {
            base.MakeDecisionCard(phaseOneOption);
            Player = player;
            Drop = dropEnum;
            CardIndex = card;
        }
    }
}