using UnityEngine;

namespace Bang
{
    public class HandCardListView : CardListView<ICardView>
    {
        protected override GameObject GetPrefab()
        {
            return GameController.Instance.CardPrefab;
        }
    }
}