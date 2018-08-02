using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bang
{

    public class PlayerView : DropView, IPlayerView
    {

        [SerializeField] private Text hp = null, info = null;
        [SerializeField] private HandView handHidden = null;
        [SerializeField] private GameObject weapon = null;
        [SerializeField] private Transform hand = null, properties = null;

        private int playerIndex;
        private int hiddenCards;
        private ICardView weaponCard;
        private List<ICardView> handCards;
        private List<ICardView> propertyCards;

        private int HiddenCards
        {
            get
            {
                return hiddenCards;
            }
            set
            {
                hiddenCards = value;
                handHidden.Text = hiddenCards.ToString();
            }
        }

        protected override void Start()
        {
            base.Start();
            handCards = new List<ICardView>();
            propertyCards = new List<ICardView>();
            weaponCard = weapon.GetComponent<ICardView>();
        }

        public void SetPlayerIndex(int index)
        {
            playerIndex = index;
        }

        public void SetSheriff()
        {
            SetRole(Roles.SHERIFF_NAME, Roles.SHERIFF_COLOR);
        }

        public void SetRole(ERole role)
        {
            Color color = Roles.GetColorFromRole(role);
            string name = Roles.GetNameFromRole(role);
            SetRole(name, color);
        }

        public void UpdateHP(int hp)
        {
            this.hp.text = hp.ToString();
        }

        private void SetRole(string name, Color color)
        {
            info.text = name;
            info.color = color;
        }

        public void AddCard()
        {
            HiddenCards += 1;
        }

        public void RemoveCard()
        {
            HiddenCards -= 1;
        }

        public void AddCard(int index, string name, ESuit suit, ERank rank, Color color)
        {
            ICardView cv = Instantiate(GameController.Instance.cardPrefab, hand);
            cv.SetIndex(index);
            cv.SetName(name, color);
            cv.SetSuit(suit);
            cv.SetRank(rank);
            handCards.Add(cv);
        }

        public void RemoveCard(int index)
        {
            ICardView cv = handCards[index];
            handCards.RemoveAt(index);
            Destroy(cv.GameObject());
            for(int i = index; i < handCards.Count; i++)
            {
                handCards[i].SetIndex(i);
            }
        }

        public void EquipWeapon(string name, ESuit suit, ERank rank, Color color)
        {
            weaponCard.SetName(name, color);
            weaponCard.SetSuit(suit);
            weaponCard.SetRank(rank);
        }

        public void EnableCard(int index, bool enable)
        {
            handCards[index].Playable(enable);
        }

        public void EnableDiscardCard(int index, bool enable)
        {
            handCards[index].Discardable(enable);
        }

        public void SetStealable(ECardDropArea cda, bool weapon)
        {
            if (handHidden) handHidden.SetDroppable(cda);
            if (weapon) weaponCard.SetDroppable(cda);
            foreach (ICardView cv in propertyCards)
                cv.SetDroppable(cda);
        }
    }
}