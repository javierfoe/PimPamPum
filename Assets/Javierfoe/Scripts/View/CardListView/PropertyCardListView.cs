using UnityEngine;

namespace Bang
{
    public class PropertyCardListView : CardListView<ICardView>
    {
        protected override GameObject GetPrefab()
        {
            return GameController.PropertyPrefab;
        }
    }
}