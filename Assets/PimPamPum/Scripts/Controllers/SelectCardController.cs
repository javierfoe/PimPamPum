using System.Collections.Generic;
using UnityEngine;

namespace PimPamPum
{
    public class SelectCardController : MonoBehaviour
    {
        [SerializeField] private GameObject selectCardGO = null;

        private ISelectCardListView selectCardList;

        public void EnableCards(bool value)
        {
            selectCardList.EnableCards(value);
        }

        public void SetCards(List<Card> cards)
        {
            int length = cards.Count;
            CardValues[] cardStructs = new CardValues[length];
            for (int i = 0; i < length; i++)
            {
                cardStructs[i] = cards[i].Struct;
            }

            AddCards(cardStructs);
        }

        public void Disable()
        {
            selectCardList.Enable(false);
        }

        public void RemoveCard(int index)
        {
            selectCardList.RemoveCard(index);
        }

        private void AddCards(CardValues[] cards)
        {
            for(int i = 0; i < cards.Length; i++)
            {
                selectCardList.AddCard(i, cards[i]);
            }
            selectCardList.Enable(true);
        }
    }
}