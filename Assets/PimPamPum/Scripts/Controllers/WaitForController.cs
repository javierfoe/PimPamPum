namespace PimPamPum
{
    public class WaitForController
    {
        private static WaitFor mainCorutine, turnCorutine, dyingCorutine;

        public static WaitFor TurnCorutine { get { return turnCorutine; } set { turnCorutine = value; value.Response = false; } }
        public static WaitFor DyingCorutine { get { return dyingCorutine; } set { dyingCorutine = value; value.Response = true; } }
        public static WaitFor MainCorutine
        {
            get { return mainCorutine; }
            set
            {
                if (TurnCorutine != null && !(TurnCorutine.Current is WaitForDying))
                {
                    TurnCorutine.Current = value;
                }
                mainCorutine = value;
                value.Response = true;
            }
        }

        public static void MakeDecision(int card)
        {
            MainCorutine.MakeDecisionCardIndex(card);
        }

        public static void MakeDecision(Decision decision, Card card = null)
        {
            WaitFor waitFor = MainCorutine;
            if (decision == Decision.Die) waitFor = DyingCorutine;
            waitFor.MakeDecisionCard(decision, card);
        }

        public static void MakeDecision(Decision phaseOne, int player, Drop dropEnum, int card)
        {
            MainCorutine.MakeDecisionPhaseOne(phaseOne, player, dropEnum, card);
        }

        public static void StopTurnCorutine()
        {
            TurnCorutine.StopCorutine();
        }

        public static void StopMainCorutine()
        {
            if (MainCorutine != null && MainCorutine != DyingCorutine) MainCorutine.StopCorutine();
        }
    }
}