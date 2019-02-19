using UnityEngine;

namespace Bang
{
    public class GeneralStoreListView : CardListView<IGeneralStoreCardView>
    {
        protected override void Awake()
        {
            base.Awake();
            gameObject.SetActive(false);
        }

        protected override GameObject GetPrefab()
        {
            return GameController.Instance.GeneralStorePrefab;
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