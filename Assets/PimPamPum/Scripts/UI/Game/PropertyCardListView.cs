using UnityEngine;

namespace PimPamPum
{
    public class PropertyCardListView : CardListView
    {
        protected override GameObject GetPrefab()
        {
            return GameController.PropertyPrefab;
        }
    }
}