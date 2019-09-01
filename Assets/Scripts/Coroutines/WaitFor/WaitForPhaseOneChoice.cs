namespace PimPamPum
{
    public class WaitForPhaseOneChoice : WaitForDecision
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

        public WaitForPhaseOneChoice(int player) : base(Decision.Deck)
        {
            this.player = player;
        }

        public override void MakeDecision(Decision phaseOneOption, int player, Drop dropEnum, int card)
        {
            base.MakeDecision(phaseOneOption);
            Player = player;
            Drop = dropEnum;
            CardIndex = card;
        }
    }
}