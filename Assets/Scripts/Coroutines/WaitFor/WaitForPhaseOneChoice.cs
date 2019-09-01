namespace PimPamPum
{
    public class WaitForPhaseOneChoice : WaitForDecision
    {
        private int player;

        public Decision PhaseOneOption
        {
            get; private set;
        }

        public int Player
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
            bool option = PhaseOneOption == Decision.Pending;
            if (!res && option)
            {
                PhaseOneOption = Decision.Deck;
                Finished();
                return false;
            }
            if (!option) Finished();
            return option;
        }

        public void Finished()
        {
            GameController.Instance.DisablePhaseOneClickable(player);
        }

        public WaitForPhaseOneChoice(int player) : base(Decision.Deck)
        {
            this.player = player;
        }

        public override void MakeDecision(Decision phaseOneOption, int player, int card)
        {
            PhaseOneOption = phaseOneOption;
            Player = player;
            CardIndex = card;
        }
    }
}