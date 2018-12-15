using System.Collections.Generic;
using UnityEngine;

namespace Bang
{
    public class CardListView : MonoBehaviour
    {

        private enum CardPrefab
        {
            Card,
            Property,
            GeneralStore
        }

        [SerializeField] private CardPrefab cardPrefab;

        private List<ICardView> list;

        private void Start()
        {
            list = new List<ICardView>();
        }

        private ICardView InstantiateCardView()
        {
            GameObject prefab = null;
            switch (cardPrefab)
            {
                case CardPrefab.Card:
                    prefab = GameController.CardPrefab;
                    break;
                case CardPrefab.Property:
                    prefab = GameController.PropertyPrefab;
                    break;
                case CardPrefab.GeneralStore:
                    prefab = GameController.CardPrefab;
                    break;
            }
            return Instantiate(prefab, transform).GetComponent<ICardView>();
        }

        public void AddCardView(int index, string name, Suit suit, Rank rank, Color color)
        {
            ICardView cv = InstantiateCardView();
            cv.SetIndex(index);
            cv.SetName(name, color);
            cv.SetSuit(suit);
            cv.SetRank(rank);
            list.Add(cv);
        }

        public void RemoveCardView(int index)
        {
            Destroy(list[index].GameObject());
            list.RemoveAt(index);
            for (int i = index; i < list.Count; i++) list[i].SetIndex(i);
        }

        public void SetDroppable(bool value)
        {
            foreach(ICardView cv in list) cv.SetDroppable(value);
        }

        public void SetPlayable(int index, bool value)
        {
            list[index].Playable(value);
        }
    }
}