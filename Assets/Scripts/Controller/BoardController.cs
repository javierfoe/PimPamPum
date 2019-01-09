using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Bang
{

    public class BoardController : NetworkBehaviour
    {
        [SerializeField] private GameObject boardViewGO = null;
        [SerializeField] private DeckDefinition deckCards;

        [System.Serializable]
        private struct DeckDefinition
        {
            public CardDefinition[] cardTypes;
        }

        [System.Serializable]
        private struct CardDefinition
        {
            public string name;
            public CardType type;
            public CardSuitRank[] suitRanks;
        }

        [System.Serializable]
        private struct CardSuitRank{
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

            CardSuitRank[] cardSuitRanks;
            foreach (CardDefinition cardDef in deckCards.cardTypes)
            {
                cardSuitRanks = cardDef.suitRanks;
                switch (cardDef.type)
                {
                    case CardType.Bang:
                        GenerateCards<Bang>(cardSuitRanks, temp);
                        break;
                    case CardType.Barrel:
                        GenerateCards<Barrel>(cardSuitRanks, temp);
                        break;
                    case CardType.Beer:
                        GenerateCards<Beer>(cardSuitRanks, temp);
                        break;
                    case CardType.Carabine:
                        GenerateCards<Carabine>(cardSuitRanks, temp);
                        break;
                    case CardType.CatBalou:
                        GenerateCards<CatBalou>(cardSuitRanks, temp);
                        break;
                    case CardType.Duel:
                        GenerateCards<Duel>(cardSuitRanks, temp);
                        break;
                    case CardType.Dynamite:
                        GenerateCards<Dynamite>(cardSuitRanks, temp);
                        break;
                    case CardType.Gatling:
                        GenerateCards<Gatling>(cardSuitRanks, temp);
                        break;
                    case CardType.GeneralStore:
                        GenerateCards<GeneralStore>(cardSuitRanks, temp);
                        break;
                    case CardType.Indians:
                        GenerateCards<Indians>(cardSuitRanks, temp);
                        break;
                    case CardType.Jail:
                        GenerateCards<Jail>(cardSuitRanks, temp);
                        break;
                    case CardType.Missed:
                        GenerateCards<Missed>(cardSuitRanks, temp);
                        break;
                    case CardType.Mustang:
                        GenerateCards<Mustang>(cardSuitRanks, temp);
                        break;
                    case CardType.Panic:
                        GenerateCards<Panic>(cardSuitRanks, temp);
                        break;
                    case CardType.Remington:
                        GenerateCards<Remington>(cardSuitRanks, temp);
                        break;
                    case CardType.Saloon:
                        GenerateCards<Saloon>(cardSuitRanks, temp);
                        break;
                    case CardType.Schofield:
                        GenerateCards<Schofield>(cardSuitRanks, temp);
                        break;
                    case CardType.Scope:
                        GenerateCards<Scope>(cardSuitRanks, temp);
                        break;
                    case CardType.Stagecoach:
                        GenerateCards<Stagecoach>(cardSuitRanks, temp);
                        break;
                    case CardType.Volcanic:
                        GenerateCards<Volcanic>(cardSuitRanks, temp);
                        break;
                    case CardType.WellsFargo:
                        GenerateCards<WellsFargo>(cardSuitRanks, temp);
                        break;
                    case CardType.Winchester:
                        GenerateCards<Winchester>(cardSuitRanks, temp);
                        break;
                }
            }

            ShuffleCards(temp);
        }

        private void GenerateCards<T>(CardSuitRank[] suitRanks, List<Card> list) where T : Card, new()
        {
            foreach (CardSuitRank suitRank in suitRanks)
            {
                list.Add(Card.CreateNew<T>(suitRank.suit, suitRank.rank));
            }
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