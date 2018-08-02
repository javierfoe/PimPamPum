using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Bang
{

    public class BoardController : NetworkBehaviour
    {
        [SerializeField] private GameObject boardViewGO = null;

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

        public Card DrawCard()
        {
            if (deck.Count < 1) ShuffleCards(discardStack);

            int index = deck.Count - 1;
            Card c = deck[index];
            deck.RemoveAt(index);
            SetDeckSize();
            return c;
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
            RpcSetDiscardTop(card.ToString(), card.Suit, card.Rank, card.Color);
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

        [ClientRpc]
        private void RpcSetDeckSize(int cards)
        {
            boardView.SetDeckSize(cards);
        }

        [ClientRpc]
        private void RpcSetDiscardTop(string name, ESuit suit, ERank rank, Color color)
        {
            boardView.SetDiscardTop(name, suit, rank, color);
        }

        [ClientRpc]
        private void RpcEmptyDiscardStack()
        {
            boardView.EmptyDiscardStack();
        }

        #region GenerateDeck
        private void GenerateDeck()
        {
            List<Card> temp = new List<Card>();

            GenerateBangs(temp);
            GenerateBarrels(temp);
            GenerateBeers(temp);
            GenerateBinoculars(temp);
            GenerateCarabines(temp);
            GenerateCatBalous(temp);
            GenerateDuels(temp);
            GenerateDynamites(temp);
            GenerateGatlings(temp);
            GenerateGeneralStores(temp);
            GenerateIndians(temp);
            GenerateJails(temp);
            GenerateMisseds(temp);
            GenerateMustangs(temp);
            GeneratePanics(temp);
            GenerateRemingtons(temp);
            GenerateSaloons(temp);
            GenerateSchofields(temp);
            GenerateStagecoaches(temp);
            GenerateVolcanics(temp);
            GenerateWellsFargos(temp);
            GenerateWinchesters(temp);

            ShuffleCards(temp);
        }

        private void GenerateBangs(List<Card> temp)
        {
            int length = Number.BANGS;
            for (int i = 0; i < length; i++)
                temp.Add(Bang.CreateBang(i));
        }

        private void GenerateMisseds(List<Card> temp)
        {
            int length = Number.MISSEDS;
            for (int i = 0; i < length; i++)
                temp.Add(Missed.CreateMissed(i));
        }

        private void GenerateBeers(List<Card> temp)
        {
            int length = Number.BEERS;
            for (int i = 0; i < length; i++)
                temp.Add(Beer.CreateBeer(i));
        }

        private void GeneratePanics(List<Card> temp)
        {
            int length = Number.PANICS;
            for (int i = 0; i < length; i++)
                temp.Add(Panic.CreatePanic(i));
        }

        private void GenerateCatBalous(List<Card> temp)
        {
            int length = Number.CAT_BALOUS;
            for (int i = 0; i < length; i++)
                temp.Add(CatBalou.CreateCatBalou(i));
        }

        private void GenerateJails(List<Card> temp)
        {
            int length = Number.JAILS;
            for (int i = 0; i < length; i++)
                temp.Add(Jail.CreateJail(i));
        }

        private void GenerateMustangs(List<Card> temp)
        {
            int length = Number.MUSTANGS;
            for (int i = 0; i < length; i++)
                temp.Add(Mustang.CreateMustang(i));
        }

        private void GenerateBarrels(List<Card> temp)
        {
            int length = Number.BARRELS;
            for (int i = 0; i < length; i++)
                temp.Add(Barrel.CreateBarrel(i));
        }

        private void GenerateStagecoaches(List<Card> temp)
        {
            int length = Number.STAGECOACHES;
            for (int i = 0; i < length; i++)
                temp.Add(new Stagecoach());
        }

        private void GenerateSchofields(List<Card> temp)
        {
            int length = Number.SCHOFIELDS;
            for (int i = 0; i < length; i++)
                temp.Add(Schofield.CreateSchofield(i));
        }

        private void GenerateVolcanics(List<Card> temp)
        {
            int length = Number.VOLCANICS;
            for (int i = 0; i < length; i++)
                temp.Add(Volcanic.CreateVolcanic(i));
        }

        private void GenerateGeneralStores(List<Card> temp)
        {
            int length = Number.GENERAL_STORES;
            for (int i = 0; i < length; i++)
                temp.Add(GeneralStore.CreateGeneralStore(i));
        }

        private void GenerateIndians(List<Card> temp)
        {
            int length = Number.INDIANS;
            for (int i = 0; i < length; i++)
                temp.Add(Indians.CreateIndians(i));
        }

        private void GenerateRemingtons(List<Card> temp)
        {
            temp.Add(new Remington());
        }

        private void GenerateCarabines(List<Card> temp)
        {
            temp.Add(new Carabine());
        }

        private void GenerateWinchesters(List<Card> temp)
        {
            temp.Add(new Winchester());
        }

        private void GenerateGatlings(List<Card> temp)
        {
            temp.Add(new Gatling());
        }

        private void GenerateWellsFargos(List<Card> temp)
        {
            temp.Add(new WellsFargo());
        }

        private void GenerateSaloons(List<Card> temp)
        {
            temp.Add(new Saloon());
        }

        private void GenerateBinoculars(List<Card> temp)
        {
            temp.Add(new Binoculars());
        }

        private void GenerateDynamites(List<Card> temp)
        {
            temp.Add(new Dynamite());
        }

        private void GenerateDuels(List<Card> temp)
        {
            int length = Number.DUELS;
            for (int i = 0; i < length; i++)
                temp.Add(Duel.CreateDuel(i));
        }
        #endregion

    }
}