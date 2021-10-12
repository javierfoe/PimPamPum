using UnityEngine;

namespace PimPamPum
{
    public class HandCardListView : CardListView
    {
        protected override GameObject GetPrefab()
        {
            return GameController.CardPrefab;
        }
    }
}