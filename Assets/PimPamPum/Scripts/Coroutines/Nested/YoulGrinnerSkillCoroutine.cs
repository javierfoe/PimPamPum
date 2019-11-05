
namespace PimPamPum
{
    public class YoulGrinnerSkillCoroutine : Enumerator
    {
        private int minimumCards, currentPlayer, startingPlayer;
        private PlayerController[] playerControllers;

        public override bool MoveNext()
        {
            WaitForCardSelection cardSelection = Current as WaitForCardSelection;
            if(cardSelection != null)
            {
                playerControllers[currentPlayer].UnequipHandCard(cardSelection.Choice);
                playerControllers[startingPlayer].AddCard(cardSelection.ChosenCard);
            }
            currentPlayer = GameController.Instance.NextPlayerAlive(currentPlayer);
            PlayerController currentPc = playerControllers[currentPlayer];
            if(startingPlayer != currentPlayer && currentPc.Hand.Count > minimumCards)
            {
                Current = new WaitForCardSelection(currentPc.connectionToClient, currentPc.Hand);
                return true;
            }
            return false;
        }

        public YoulGrinnerSkillCoroutine(int player, PlayerController[] playerControllers)
        {
            this.playerControllers = playerControllers;
            startingPlayer = player;
            minimumCards = playerControllers[player].Hand.Count;
            currentPlayer = player;
        }
    }
}