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

            RpcEnableGeneralStore(true);
            RpcEnableCards(false);
            Card c;
            for (int i = 0; i < result.Count; i++)
            {
                c = result[i];
                RpcAddCardGeneralStore(i, c.Struct);
            }

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

        public void ShowBangEvent(BangEvent bangEvent)
        {
            RpcShowBangEvent(bangEvent);
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
        private void RpcShowBangEvent(BangEvent bangEvent)
        {
            boardView.ShowBangEvent(bangEvent);
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

        #region GenerateDeck
        private void GenerateDeck()
        {
            List<Card> temp = new List<Card>();

            GenerateBangs(temp);
            GenerateBarrels(temp);
            GenerateBeers(temp);
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
            GenerateScopes(temp);
            GenerateStagecoaches(temp);
            GenerateVolcanics(temp);
            GenerateWellsFargos(temp);
            GenerateWinchesters(temp);

            ShuffleCards(temp);
        }

        private void GenerateBangs(List<Card> temp)
        {
            int length = Number.Bangs;
            for (int i = 0; i < length; i++)
                temp.Add(Bang.CreateBang(i));
        }

        private void GenerateMisseds(List<Card> temp)
        {
            int length = Number.Misseds;
            for (int i = 0; i < length; i++)
                temp.Add(Missed.CreateMissed(i));
        }

        private void GenerateBeers(List<Card> temp)
        {
            int length = Number.Beers;
            for (int i = 0; i < length; i++)
                temp.Add(Beer.CreateBeer(i));
        }

        private void GeneratePanics(List<Card> temp)
        {
            int length = Number.Panics;
            for (int i = 0; i < length; i++)
                temp.Add(Panic.CreatePanic(i));
        }

        private void GenerateCatBalous(List<Card> temp)
        {
            int length = Number.CatBalous;
            for (int i = 0; i < length; i++)
                temp.Add(CatBalou.CreateCatBalou(i));
        }

        private void GenerateJails(List<Card> temp)
        {
            int length = Number.Jails;
            for (int i = 0; i < length; i++)
                temp.Add(Jail.CreateJail(i));
        }

        private void GenerateMustangs(List<Card> temp)
        {
            int length = Number.Mustangs;
            for (int i = 0; i < length; i++)
                temp.Add(Mustang.CreateMustang(i));
        }

        private void GenerateBarrels(List<Card> temp)
        {
            int length = Number.Barrels;
            for (int i = 0; i < length; i++)
                temp.Add(Barrel.CreateBarrel(i));
        }

        private void GenerateStagecoaches(List<Card> temp)
        {
            int length = Number.Stagecoaches;
            for (int i = 0; i < length; i++)
                temp.Add(new Stagecoach());
        }

        private void GenerateSchofields(List<Card> temp)
        {
            int length = Number.Schofields;
            for (int i = 0; i < length; i++)
                temp.Add(Schofield.CreateSchofield(i));
        }

        private void GenerateVolcanics(List<Card> temp)
        {
            int length = Number.Volcanics;
            for (int i = 0; i < length; i++)
                temp.Add(Volcanic.CreateVolcanic(i));
        }

        private void GenerateGeneralStores(List<Card> temp)
        {
            int length = Number.GeneralStores;
            for (int i = 0; i < length; i++)
                temp.Add(GeneralStore.CreateGeneralStore(i));
        }

        private void GenerateIndians(List<Card> temp)
        {
            int length = Number.Indians;
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

        private void GenerateScopes(List<Card> temp)
        {
            temp.Add(new Scope());
        }

        private void GenerateDynamites(List<Card> temp)
        {
            temp.Add(new Dynamite());
        }

        private void GenerateDuels(List<Card> temp)
        {
            int length = Number.Duels;
            for (int i = 0; i < length; i++)
                temp.Add(Duel.CreateDuel(i));
        }
        #endregion

    }
}