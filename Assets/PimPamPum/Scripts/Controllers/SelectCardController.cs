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
            Card c;
            for (int i = 0; i < cards.Count; i++)
            {
                c = cards[i];
                AddCard(i, c.Struct, conn);
            }

            Enable(true, conn);

            if (conn != null)
            {
                TargetEnableCards(conn, true);
            }
        }

        private void AddCard(int index, CardStruct card, NetworkConnection conn)
        {
            if(conn == null)
            {
                RpcAddCard(index, card);
            }
            else
            {
                TargetAddCard(conn, index, card);
            }
        }

        private void Enable(bool value, NetworkConnection conn)
        {
            if(conn == null)
            {
                RpcEnable(value);
            }
            else
            {
                TargetEnable(conn, value);
            }
        }

        public void Disable()
        {
            RpcEnable(false);
        }

        public void RemoveCardsAndDisable(NetworkConnection conn)
        {
            TargetRemoveCardsAndDisable(conn);
        }

        public void RemoveCard(int index)
        {
            RpcRemoveCard(index);
        }

        [TargetRpc]
        private void TargetEnableCards(NetworkConnection conn, bool value)
        {
            selectCardList.EnableCards(value);
        }

        [TargetRpc]
        private void TargetEnable(NetworkConnection conn, bool value)
        {
            selectCardList.Enable(value);
        }

        [TargetRpc]
        private void TargetAddCard(NetworkConnection conn, int index, CardStruct cs)
        {
            selectCardList.AddCard(index, cs);
        }

        [TargetRpc]
        private void TargetRemoveCardsAndDisable(NetworkConnection conn)
        {
            selectCardList.RemoveAllCards();
            selectCardList.Enable(false);
        }

        [ClientRpc]
        private void RpcEnableCards(bool value)
        {
            selectCardList.EnableCards(value);
        }

        [ClientRpc]
        private void RpcEnable(bool value)
        {
            selectCardList.Enable(value);
        }

        [ClientRpc]
        private void RpcAddCard(int index, CardStruct cs)
        {
            selectCardList.AddCard(index, cs);
        }

        [ClientRpc]
        private void RpcRemoveCard(int index)
        {
            selectCardList.RemoveCard(index);
        }
    }
}