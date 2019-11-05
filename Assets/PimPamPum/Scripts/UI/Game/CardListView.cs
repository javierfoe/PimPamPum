using System.Collections.Generic;
using UnityEngine;

namespace PimPamPum
{
    public abstract class CardListView : MonoBehaviour, ICardListView
    {

        protected List<ICardView> list;

        protected virtual void Awake()
        {
            list = new List<ICardView>();
        }

        protected abstract GameObject GetPrefab();

        public void AddCard(int index, CardStruct cs, IPlayerView iPlayerView = null)
        {
            GameObject prefab = GetPrefab();
            ICardView cv = Instantiate(prefab, transform).GetComponent<ICardView>();
            cv.SetIndex(index);
            cv.SetCard(cs);
            cv.IPlayerView = iPlayerView;
            list.Add(cv);
        }

        public void RemoveCard(int index)
        {
            Destroy(list[index].gameObject);
            list.RemoveAt(index);
            for (int i = index; i < list.Count; i++) list[i].SetIndex(i);
        }

        public void RemoveAllCards()
        {
            for(int i = 0; i < list.Count; i++)
            {
                Destroy(list[i].gameObject);
            }
            list.Clear();
        }

        public void SetDroppable(bool value)
        {
            foreach(ICardView cv in list) cv.Droppable = value;
        }

        public void SetPlayable(int index, bool value)
        {
            list[index].Playable(value);
        }

        public void SetClickable(bool value)
        {
            foreach (ICardView cv in list) cv.EnableClick(value);
        }
    }
}