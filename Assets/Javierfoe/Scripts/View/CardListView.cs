using System.Collections.Generic;
using UnityEngine;

namespace Bang
{
    public abstract class CardListView<T> : MonoBehaviour where T : ICardView
    {

        protected List<T> list;

        private void Start()
        {
            list = new List<T>();
        }

        protected abstract GameObject GetPrefab();

        public void AddCardView(int index, string name, Suit suit, Rank rank, Color color)
        {
            GameObject prefab = GetPrefab();
            T cv = Instantiate(prefab, transform).GetComponent<T>();
            cv.SetIndex(index);
            cv.SetName(name, color);
            cv.SetSuit(suit);
            cv.SetRank(rank);
            list.Add(cv);
        }

        public void RemoveCardView(int index)
        {
            Destroy(list[index].GameObject());
            list.RemoveAt(index);
            for (int i = index; i < list.Count; i++) list[i].SetIndex(i);
        }

        public void SetDroppable(bool value)
        {
            foreach(T cv in list) cv.SetDroppable(value);
        }

        public void SetPlayable(int index, bool value)
        {
            list[index].Playable(value);
        }
    }
}