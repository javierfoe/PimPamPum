using System.Collections.Generic;

namespace PimPamPum
{
    public class EvelynShebangSkillCoroutine : Enumerator
    {
        private PlayerController player;
        private List<int> availablePlayers;
        private int playerNumber, cards;

        public override bool MoveNext()
        {
            WaitForPhaseOneChoice waitForPhaseOne = Current as WaitForPhaseOneChoice;
            if(waitForPhaseOne != null)
            {
                cards--;
                switch (waitForPhaseOne.Decision)
                {
                    case Decision.Deck:
                        player.Draw();
                        break;
                    case Decision.Player:
                        int targetPlayer = waitForPhaseOne.Player;
                        availablePlayers.Remove(targetPlayer);
                        Current = GameController.Instance.PimPamPum(playerNumber, targetPlayer);
                        return true;
                }
            }
            if(availablePlayers.Count > 0 && cards > 0)
            {
                GameController.Instance.SetPhaseOnePlayerClickable(playerNumber, availablePlayers);
                Current = new WaitForPhaseOneChoice(playerNumber);
                return true;
            }
            player.Draw(cards);
            return false;
        }

        public EvelynShebangSkillCoroutine(PlayerController player, int cards)
        {
            this.player = player;
            playerNumber = player.PlayerNumber;
            this.cards = cards;
            availablePlayers = GameController.Instance.PlayersInWeaponRange(playerNumber);
        }
    }
}