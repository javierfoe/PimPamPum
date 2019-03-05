using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Bang
{

    public class BoardController : NetworkBehaviour
    {
        [SerializeField] private GameObject boardViewGO = null;
        [SerializeField] private CardDefinition[] deckCards = null;

        [System.Serializable]
        private struct CardDefinition
        {
            public CardType type;
            public Suit suit;
            public Rank rank;
        }

        private IBoardView boardView;
        private List<Card> deck;
        private List<Card> discardStack;

        public override void OnStartClient()
        {
            boardView = boardViewGO.GetComponent<IBoardView>();
        }

        public int DeckSize
        {
            get { return deck.Count; }
        }

        public Card DiscardStackTop
        {
            get { return discardStack[discardStack.Count - 1]; }
        }

        public void ConstructorBoard()
        {
            deck = new List<Card>();
            discardStack = new List<Card>();

            GenerateDeck();
        }

        public void EnableCards(NetworkConnection conn, bool value)
        {
            TargetEnableCards(conn, value);
        }

        public Card DrawCard()
        {
            if (deck.Count < 1) ShuffleCards(discardStack);

            int index = deck.Count - 1;
            Card c = deck[index];
            deck.RemoveAt(index);
            SetDeckSize();
            return c;
        }

        public List<Card> DrawGeneralStoreCards(int cards)
        {
            List<Card> result = DrawCards(cards);

            Card c;
            for (int i = 0; i < result.Count; i++)
            {
                c = result[i];
                RpcAddCardGeneralStore(i, c.Struct);
            }

            RpcEnableGeneralStore(true);
            RpcEnableCards(false);

            return result;
        }

        public void DisableGeneralStore()
        {
            RpcEnableGeneralStore(false);
        }

        public void RemoveGeneralStoreCard(int index)
        {
            RpcRemoveCardGeneralStore(index);
        }

        public List<Card> DrawCards(int cards)
        {
            List<Card> result = new List<Card>();
            for (int i = 0; i < cards; i++)
                result.Add(DrawCard());
            return result;
        }

        public void DiscardCard(Card card)
        {
            discardStack.Add(card);
            RpcSetDiscardTop(card.Struct);
        }

        public void ShuffleCards(List<Card> temp)
        {
            Card c;
            int random;
            while (temp.Count > 0)
            {
                random = Random.Range(0, temp.Count);
                c = temp[random];
                temp.RemoveAt(random);
                deck.Add(c);
            }
            RpcEmptyDiscardStack();
        }

        private void SetDeckSize()
        {
            RpcSetDeckSize(deck.Count);
        }

        public void SetTargetable(NetworkConnection conn, bool value)
        {
            TargetTargetableTrash(conn, value);
        }

        private void GenerateDeck()
        {
            List<Card> temp = new List<Card>();

            foreach (CardDefinition card in deckCards)
            {
                GenerateCard(card, temp);
            }

            ShuffleCards(temp);
        }

        private void GenerateCard(CardDefinition card, List<Card> list)
        {
            Card c = null;
            Suit suit = card.suit;
            Rank rank = card.rank;
            switch (card.type)
            {
                case CardType.Bang:
                    c = Card.CreateNew<Bang>(suit, rank);
                    break;
                case CardType.Barrel:
                    c = Card.CreateNew<Barrel>(suit, rank);
                    break;
                case CardType.Beer:
                    c = Card.CreateNew<Beer>(suit, rank);
                    break;
                case CardType.Carabine:
                    c = Card.CreateNew<Carabine>(suit, rank);
                    break;
                case CardType.CatBalou:
                    c = Card.CreateNew<CatBalou>(suit, rank);
                    break;
                case CardType.Duel:
                    c = Card.CreateNew<Duel>(suit, rank);
                    break;
                case CardType.Dynamite:
                    c = Card.CreateNew<Dynamite>(suit, rank);
                    break;
                case CardType.Gatling:
                    c = Card.CreateNew<Gatling>(suit, rank);
                    break;
                case CardType.GeneralStore:
                    c = Card.CreateNew<GeneralStore>(suit, rank);
                    break;
                case CardType.Indians:
                    c = Card.CreateNew<Indians>(suit, rank);
                    break;
                case CardType.Jail:
                    c = Card.CreateNew<Jail>(suit, rank);
                    break;
                case CardType.Missed:
                    c = Card.CreateNew<Missed>(suit, rank);
                    break;
                case CardType.Mustang:
                    c = Card.CreateNew<Mustang>(suit, rank);
                    break;
                case CardType.Panic:
                    c = Card.CreateNew<Panic>(suit, rank);
                    break;
                case CardType.Remington:
                    c = Card.CreateNew<Remington>(suit, rank);
                    break;
                case CardType.Saloon:
                    c = Card.CreateNew<Saloon>(suit, rank);
                    break;
                case CardType.Schofield:
                    c = Card.CreateNew<Schofield>(suit, rank);
                    break;
                case CardType.Scope:
                    c = Card.CreateNew<Scope>(suit, rank);
                    break;
                case CardType.Stagecoach:
                    c = Card.CreateNew<Stagecoach>(suit, rank);
                    break;
                case CardType.Volcanic:
                    c = Card.CreateNew<Volcanic>(suit, rank);
                    break;
                case CardType.WellsFargo:
                    c = Card.CreateNew<WellsFargo>(suit, rank);
                    break;
                case CardType.Winchester:
                    c = Card.CreateNew<Winchester>(suit, rank);
                    break;
            }
            if (c != null) list.Add(c);
        }

        [TargetRpc]
        private void TargetTargetableTrash(NetworkConnection conn, bool value)
        {
            boardView.SetTargetable(value);
        }

        [TargetRpc]
        private void TargetEnableCards(NetworkConnection conn, bool value)
        {
            boardView.EnableGeneralStoreCards(value);
        }

        [ClientRpc]
        private void RpcEnableCards(bool value)
        {
            boardView.EnableGeneralStoreCards(value);
        }

        [ClientRpc]
        private void RpcEnableGeneralStore(bool value)
        {
            boardView.EnableGeneralStore(value);
        }

        [ClientRpc]
        private void RpcAddCardGeneralStore(int index, CardStruct cs)
        {
            boardView.AddGeneralStoreCard(index, cs);
        }

        [ClientRpc]
        private void RpcRemoveCardGeneralStore(int index)
        {
            boardView.RemoveGeneralStoreCard(index);
        }

        [ClientRpc]
        private void RpcSetDeckSize(int cards)
        {
            boardView.SetDeckSize(cards);
        }

        [ClientRpc]
        private void RpcSetDiscardTop(CardStruct cs)
        {
            boardView.SetDiscardTop(cs);
        }

        [ClientRpc]
        private void RpcEmptyDiscardStack()
        {
            boardView.EmptyDiscardStack();
        }

    }
}