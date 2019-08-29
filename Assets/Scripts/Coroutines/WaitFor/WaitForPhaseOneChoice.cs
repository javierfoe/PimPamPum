namespace PimPamPum
{
    public class WaitForPhaseOneChoice : WaitForTimer
    {
        private int player;

        public PhaseOneOption PhaseOneOption
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
            bool option = PhaseOneOption == PhaseOneOption.Nothing;
            if (!res && option)
            {
                PhaseOneOption = PhaseOneOption.Deck;
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

        public WaitForPhaseOneChoice(int player)
        {
            this.player = player;
            PhaseOneOption = PhaseOneOption.Nothing;
        }

        public override void MakeDecision(PhaseOneOption phaseOneOption, int player, int card)
        {
            PhaseOneOption = phaseOneOption;
            Player = player;
            CardIndex = card;
        }
    }
}