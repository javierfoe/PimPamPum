using System.Collections.Generic;
using UnityEngine;
using Mirror;
namespace PimPamPum
{
    public class BoardController : NetworkBehaviour
    {
        [SyncVar(hook = nameof(UpdateDeckSize))] private int deckSize;
        [SyncVar(hook = nameof(UpdateDiscardTopCard))] private CardValues discardTopCard;

        [SerializeField] private GameObject boardViewGO = null, deckViewGO = null, discardViewGO = null;
        [SerializeField] private CardDefinition[] deckCards = null;

        [System.Serializable]
        public struct CardDefinition
        {
            public CardType type;
            public Suit suit;
            public Rank rank;
        }

        private IDropView boardView;
        private IDeckView deckView;
        private IDiscardView discardView;
        private List<Card> deck;
        private List<Card> discardStack;

        public int DiscardStackSize => discardStack.Count;
        private Card DiscardStackTop => discardStack[discardStack.Count - 1];

        public override void OnStartClient()
        {
            boardView = boardViewGO.GetComponent<IDropView>();
            deckView = deckViewGO.GetComponent<IDeckView>();
            discardView = discardViewGO.GetComponent<IDiscardView>();
        }

        public void ConstructorBoard()
        {
            deck = new List<Card>();
            discardStack = new List<Card>();

            GenerateDeck();
        }

        public void EnableClickableDeck(NetworkConnection conn, bool value)
        {
            TargetEnableClickableDeck(conn, value);
        }

        public void EnableClickableDiscard(NetworkConnection conn, bool value)
        {
            TargetEnableClickableDiscard(conn, value);
        }

        public Card DrawCard()
        {
            if (deck.Count < 1) ShuffleCards(discardStack);

            int index = deck.Count - 1;
            Card c = deck[index];
            deck.RemoveAt(index);
            deckSize--;
            return c;
        }

        public void AddCardToDeck(Card c)
        {
            deck.Add(c);
            deckSize++;
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
            discardTopCard = card.Struct;
        }

        public Card GetDiscardTopCard()
        {
            Card res = DiscardStackTop;
            discardStack.RemoveAt(discardStack.Count - 1);
            discardTopCard = DiscardStackTop.Struct;
            return res;
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
            discardTopCard = CardValues.Null;
        }

        public void SetTargetable(bool value)
        {
            boardView.SetTargetable(value);
        }

        private void GenerateDeck()
        {
            List<Card> temp = new List<Card>();

            foreach (CardDefinition card in deckCards)
            {
                GenerateCard(card, temp);
            }

            ShuffleCards(temp);
            deckSize = deckCards.Length;
        }

        private void GenerateCard(CardDefinition card, List<Card> list)
        {
            Card c = null;
            Suit suit = card.suit;
            Rank rank = card.rank;
            switch (card.type)
            {
                case CardType.PimPamPum:
                    c = Card.CreateNew<PimPamPum>(suit, rank);
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

        private void UpdateDeckSize(int deckSize)
        {
            deckView.SetDeckSize(deckSize);
        }

        private void UpdateDiscardTopCard(CardValues discardCard)
        {
            discardView.SetDiscardTop(discardCard);
        }

        [Client]
        private void EnableClickableDeck(bool value)
        {
            deckView.EnableClick(value);
        }

        [Client]
        private void EnableClickableDiscard(bool value)
        {
            discardView.EnableClick(value);
            EnableClickableDeck(value);
        }

        [TargetRpc]
        private void TargetEnableClickableDeck(NetworkConnection conn, bool value)
        {
            EnableClickableDeck(value);
        }

        [TargetRpc]
        private void TargetEnableClickableDiscard(NetworkConnection conn, bool value)
        {
            EnableClickableDiscard(value);
        }
    }
}