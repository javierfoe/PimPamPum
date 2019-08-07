using System.Collections;
using System.Collections.Generic;

namespace PimPamPum
{

    public class DrawEffectCoroutine : IEnumerator
    {

        public Card DrawEffectCard { get; private set; }

        public object Current { get; private set; }

        private List<Card> drawnCards;
        private int player, currentCard, maxCards;
        private bool drawEffectStarted;

        public bool MoveNext()
        {
            ChooseCardTimer chooseCardTimer = Current as ChooseCardTimer;
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
                Current = GameController.Instance.DrawEffect(player, drawnCards[currentCard++]);
                if (currentCard == maxCards) return false;
            }
            return true;
        }

        public void Reset() { }

        public DrawEffectCoroutine(PlayerController pc, float decisionTime)
        {
            player = pc.PlayerNumber;
            if (pc.DrawEffectCards < 2)
            {
                DrawEffectCard = GameController.Instance.DrawCard();
                drawnCards = new List<Card>();
                drawnCards.Add(DrawEffectCard);
            }
            else
            {
                Current = new ChooseCardTimer(pc.connectionToClient, pc.DrawEffectCards, decisionTime);
            }
        }
    }

}