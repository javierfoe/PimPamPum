using UnityEngine;

namespace Bang
{
    public class GeneralStoreCardListView : CardListView<IGeneralStoreCardView>
    {
        protected override GameObject GetPrefab()
        {
            return GameController.GeneralStorePrefab;
        }

        public void EnableCards(bool value)
        {
            foreach(IGeneralStoreCardView cv in list)
            {
                cv.Enable(value);
            }
        }
    }
}