using UnityEngine;

namespace PimPamPum
{
    public class SelectCardListView : CardListView, ISelectCardListView
    {
        protected override void Awake()
        {
            base.Awake();
            gameObject.SetActive(false);
        }

        protected override GameObject GetPrefab()
        {
            return GameController.CardPrefab;
        }

        public void EnableCards(bool value)
        {
            foreach(ICardView cv in list)
            {
                cv.Selectable(value);
            }
        }

        public void Enable(bool value)
        {
            gameObject.SetActive(value);
            if (!value) RemoveAllCards();
        }

    }
}