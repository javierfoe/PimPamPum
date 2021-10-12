using System.Collections.Generic;

namespace PimPamPum
{

    public class DrawEffectCoroutine : Enumerator
    {

        public Card DrawEffectCard { get; private set; }

        private List<Card> drawnCards;
        private int player, currentCard, maxCards;
        private bool drawEffectStarted;

        public override bool MoveNext()
        {
            WaitForCardSelection chooseCardTimer = Current as WaitForCardSelection;
            if(chooseCardTimer != null && chooseCardTimer.ChosenCard != null)
            {
                DrawEffectCard = chooseCardTimer.ChosenCard;
                drawnCards = chooseCardTimer.Cards;
            }
            if(drawnCards != null && !drawEffectStarted)
            {
                currentCard = 0;
                maxCards = drawnCards.Count;
                drawEffectStarted = true;
            }
            if (drawEffectStarted && currentCard < maxCards)
            {
                Current = GameController.DrawEffect(player, drawnCards[currentCard++]);
                return true;
            }
            if (maxCards > 0 && currentCard == maxCards) return false;
            return true;
        }

        public DrawEffectCoroutine(PlayerController pc)
        {
            player = pc.PlayerNumber;
            if (pc.DrawEffectCards < 2)
            {
                DrawEffectCard = GameController.DrawCard();
                drawnCards = new List<Card>();
                drawnCards.Add(DrawEffectCard);
            }
            else
            {
                Current = new WaitForCardSelection(pc, pc.DrawEffectCards);
            }
        }
    }

}