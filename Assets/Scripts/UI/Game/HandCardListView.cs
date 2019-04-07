using UnityEngine;

namespace PimPamPum
{
    public class HandCardListView : CardListView<ICardView>
    {
        protected override GameObject GetPrefab()
        {
            return GameController.Instance.CardPrefab;
        }
    }
}