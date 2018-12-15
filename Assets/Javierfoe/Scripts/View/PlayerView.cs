using UnityEngine;
using UnityEngine.UI;

namespace Bang
{

    public class PlayerView : DropView, IPlayerView
    {

        [SerializeField] private Text hp = null, info = null;
        [SerializeField] private HandView handHidden = null;
        [SerializeField] private GameObject weapon = null;
        [SerializeField] private CardListView handCards = null, propertyCards = null;

        private int playerIndex;
        private int hiddenCards;
        private ICardView weaponCard;
        private TakeHitButton takeHitButton;
        private EndTurnButton endTurnButton;
        private DieButton dieButton;

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
            weaponCard = weapon.GetComponent<ICardView>();
        }

        public void SetClientButtons()
        {
            endTurnButton = FindObjectOfType<EndTurnButton>();
            takeHitButton = FindObjectOfType<TakeHitButton>();
            dieButton = FindObjectOfType<DieButton>();
            endTurnButton.Active = false;
            takeHitButton.Active = false;
            dieButton.Active = false;
        }

        public void SetPlayerIndex(int index)
        {
            playerIndex = index;
        }

        public int GetPlayerIndex()
        {
            return playerIndex;
        }

        public void SetSheriff()
        {
            SetRole(Roles.SheriffName, Roles.SheriffColor);
        }

        public void SetRole(Role role)
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

        public void AddCard(int index, string name, Suit suit, Rank rank, Color color)
        {
            handCards.AddCardView(index, name, suit, rank, color);
        }

        public void EquipProperty(int index, string name, Suit suit, Rank rank, Color color)
        {
            propertyCards.AddCardView(index, name, suit, rank, color);
        }

        public void RemoveCard(int index)
        {
            handCards.RemoveCardView(index);
        }

        public void RemoveProperty(int index)
        {
            propertyCards.RemoveCardView(index);
        }

        public void EquipWeapon(string name, Suit suit, Rank rank, Color color)
        {
            weaponCard.SetName(name, color);
            weaponCard.SetSuit(suit);
            weaponCard.SetRank(rank);
        }

        public void EnableCard(int index, bool enable)
        {
            handCards.SetPlayable(index, enable);
        }

        public void SetStealable(bool value, bool weapon)
        {
            if (handHidden.gameObject.activeSelf)
            {
                handHidden.SetDroppable(value);
            }
            else
            {
                handCards.SetDroppable(value);
            }
            if (weapon) weaponCard.SetDroppable(value);
            propertyCards.SetDroppable(value);
        }

        public void EnableEndTurnButton(bool value)
        {
            endTurnButton.Active = value;
        }

        public void EnableTakeHitButton(bool value)
        {
            takeHitButton.Active = value;
        }

        public void EnableDieButton(bool value)
        {
            dieButton.Active = value;
        }
    }
}