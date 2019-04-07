using UnityEngine;

namespace PimPamPum
{
    public class PropertyCardListView : CardListView<ICardView>
    {
        protected override GameObject GetPrefab()
        {
            return GameController.Instance.PropertyPrefab;
        }
    }
}