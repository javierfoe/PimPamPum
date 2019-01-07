using UnityEngine;
using UnityEngine.UI;

namespace Bang
{

    public class PlayerView : DropView, IPlayerView
    {

        [SerializeField] private Text hp = null, playerName = null, info = null;
        [SerializeField] private HandView handHidden = null;
        [SerializeField] private GameObject weapon = null, turn = null;
        [SerializeField] private HandCardListView handCards = null;
        [SerializeField] private PropertyCardListView propertyCards = null;

        private int playerIndex;
        private int hiddenCards;
        private ICardView weaponCard;
        private EndGamePanelView endGamePanel;
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

        protected override void Awake()
        {
            base.Awake();
            SetTurn(false);
            weaponCard = weapon.GetComponent<ICardView>();
        }

        public void SetLocalPlayer()
        {
            endTurnButton = FindObjectOfType<EndTurnButton>();
            takeHitButton = FindObjectOfType<TakeHitButton>();
            dieButton = FindObjectOfType<DieButton>();
            endGamePanel = FindObjectOfType<EndGamePanelView>();
            endGamePanel.gameObject.SetActive(false);
            endTurnButton.Active = false;
            takeHitButton.Active = false;
            dieButton.Active = false;
        }

        public void SetTurn(bool value)
        {
            turn.SetActive(value);
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
            SetRole(Role.Sheriff);
        }

        public void SetRole(Role role)
        {
            Color color = Roles.GetColorFromRole(role);
            string name = Roles.GetNameFromRole(role);
            info.text = name;
            info.color = color;
        }

        public void UpdateHP(int hp)
        {
            this.hp.text = hp.ToString();
        }

        public void AddCard()
        {
            HiddenCards += 1;
        }

        public void RemoveCard()
        {
            HiddenCards -= 1;
        }

        public void AddCard(int index, CardStruct cs)
        {
            handCards.AddCardView(index, cs);
        }

        public void EquipProperty(int index, CardStruct cs)
        {
            propertyCards.AddCardView(index, cs);
        }

        public void RemoveCard(int index)
        {
            handCards.RemoveCardView(index);
        }

        public void RemoveProperty(int index)
        {
            propertyCards.RemoveCardView(index);
        }

        public void EquipWeapon(CardStruct cs)
        {
            weaponCard.SetCard(cs);
        }

        public void EnableCard(int index, bool enable)
        {
            handCards.SetPlayable(index, enable);
        }

        public void SetStealable(bool value, bool hand, bool weapon)
        {
            SetTargetable(value);
            if (!value || hand)
            {
                if (handHidden.gameObject.activeSelf)
                {
                    handHidden.SetDroppable(value);
                }
                else
                {
                    handCards.SetDroppable(value);
                }
            }
            if (!value || weapon) weaponCard.SetDroppable(value);
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

        public void Win()
        {
            endGamePanel.Win();
        }

        public void Lose()
        {
            endGamePanel.Lose();
        }

        public void SetPlayerName(string name)
        {
            playerName.text = name;
        }
    }
}