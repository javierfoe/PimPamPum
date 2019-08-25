using System.Collections.Generic;
using UnityEngine;

namespace PimPamPum
{
    public abstract class CardListView : MonoBehaviour
    {

        protected List<ICardView> list;

        protected virtual void Awake()
        {
            list = new List<ICardView>();
        }

        protected abstract GameObject GetPrefab();

        public void AddCard(int index, CardStruct cs)
        {
            GameObject prefab = GetPrefab();
            ICardView cv = Instantiate(prefab, transform).GetComponent<ICardView>();
            cv.SetIndex(index);
            cv.SetCard(cs);
            list.Add(cv);
        }

        public void RemoveCard(int index)
        {
            Destroy(list[index].GameObject());
            list.RemoveAt(index);
            for (int i = index; i < list.Count; i++) list[i].SetIndex(i);
        }

        public void RemoveAllCards()
        {
            for(int i = 0; i < list.Count; i++)
            {
                Destroy(list[i].GameObject());
            }
            list.Clear();
        }

        public void SetDroppable(bool value)
        {
            foreach(ICardView cv in list) cv.SetDroppable(value);
        }

        public void SetPlayable(int index, bool value)
        {
            list[index].Playable(value);
        }
    }
}