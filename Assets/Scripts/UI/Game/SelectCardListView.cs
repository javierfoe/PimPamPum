using UnityEngine;

namespace PimPamPum
{
    public class SelectCardListView : CardListView<ISelectView>, ISelectCardListView
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
            foreach(ISelectView cv in list)
            {
                cv.Enable(value);
            }
        }

        public void Enable(bool value)
        {
            gameObject.SetActive(value);
        }

    }
}