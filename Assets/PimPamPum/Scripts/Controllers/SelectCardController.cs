using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace PimPamPum
{
    public class SelectCardController : NetworkBehaviour
    {
        [SerializeField] private GameObject selectCardGO = null;

        private ISelectCardListView selectCardList;

        public override void OnStartClient()
        {
            selectCardList = selectCardGO.GetComponent<ISelectCardListView>();
        }

        public void EnableCards(NetworkConnection conn, bool value)
        {
            TargetEnableCards(conn, value);
        }

        public void SetCards(List<Card> cards, NetworkConnection conn = null)
        {
            int length = cards.Count;
            CardStruct[] cardStructs = new CardStruct[length];
            for (int i = 0; i < length; i++)
            {
                cardStructs[i] = cards[i].Struct;
            }

            AddCards(cardStructs, conn);

            if (conn != null)
            {
                TargetEnableCards(conn, true);
            }
        }

        private void AddCards(CardStruct[] cards, NetworkConnection conn)
        {
            if(conn == null)
            {
                RpcAddCards(cards);
            }
            else
            {
                TargetAddCards(conn, cards);
            }
        }

        public void Disable(NetworkConnection conn = null)
        {
            if (conn == null)
            {
                RpcEnable(false);
            }
            else
            {
                TargetDisable(conn);
            }
        }

        public void RemoveCard(int index)
        {
            RpcRemoveCard(index);
        }

        private void AddCards(CardStruct[] cards)
        {
            for(int i = 0; i < cards.Length; i++)
            {
                selectCardList.AddCard(i, cards[i]);
            }
            selectCardList.Enable(true);
        }

        [TargetRpc]
        private void TargetEnableCards(NetworkConnection conn, bool value)
        {
            selectCardList.EnableCards(value);
        }

        [TargetRpc]
        private void TargetAddCards(NetworkConnection conn, CardStruct[] cards)
        {
            AddCards(cards);
        }

        [TargetRpc]
        private void TargetDisable(NetworkConnection conn)
        {
            selectCardList.Enable(false);
        }

        [ClientRpc]
        private void RpcEnable(bool value)
        {
            selectCardList.Enable(value);
        }

        [ClientRpc]
        private void RpcAddCards(CardStruct[] cards)
        {
            AddCards(cards);
        }

        [ClientRpc]
        private void RpcRemoveCard(int index)
        {
            selectCardList.RemoveCard(index);
        }
    }
}