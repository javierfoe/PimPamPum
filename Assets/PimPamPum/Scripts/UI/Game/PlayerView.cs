using UnityEngine;
using UnityEngine.UI;

namespace PimPamPum
{

    public class PlayerView : DropView, IPlayerView
    {

        [SerializeField] private Text hp = null, playerName = null, info = null, character = null;
        [SerializeField]
        private GameObject
            handHiddenGO = null,
            weaponGO = null,
            handCardsGO = null,
            propertyCardsGO = null,
            skillGO = null,
            turnCountdownGO = null,
            responseCountdownGO = null,
            turn = null;

        private int hiddenCards;
        private IHandView handHidden;
        private ICardView weaponCard;
        private ISkillView skill;
        private ICardListView handCards, propertyCards;
        private ICountdownView turnCountdown, responseCountdown;
        private EndGamePanelView endGamePanel;
        private TakeHitButton takeHitButton;
        private EndTurnButton endTurnButton;
        private DieButton dieButton;
        private BarrelButton barrelButton;
        private SkipButton passButton;
        private CancelButton cancelButton;
        private ConfirmButton confirmButton;

        public override int PlayerNumber => PlayerIndex;

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

        public int PlayerIndex
        {
            get; set;
        }

        protected override void Awake()
        {
            weaponCard = weaponGO.GetComponent<ICardView>();
            handCards = handCardsGO.GetComponent<ICardListView>();
            propertyCards = propertyCardsGO.GetComponent<ICardListView>();
            handHidden = handHiddenGO.GetComponent<IHandView>();
            skill = skillGO.GetComponent<ISkillView>();
            turnCountdown = turnCountdownGO.GetComponent<ICountdownView>();
            responseCountdown = responseCountdownGO.GetComponent<ICountdownView>();
            turnCountdownGO.SetActive(false);
            responseCountdownGO.SetActive(false);
            base.Awake();
            SetTurn(false);
        }

        public void SetLocalPlayer()
        {
            endTurnButton = FindObjectOfType<EndTurnButton>();
            takeHitButton = FindObjectOfType<TakeHitButton>();
            dieButton = FindObjectOfType<DieButton>();
            endGamePanel = FindObjectOfType<EndGamePanelView>();
            barrelButton = FindObjectOfType<BarrelButton>();
            passButton = FindObjectOfType<SkipButton>();
            cancelButton = FindObjectOfType<CancelButton>();
            confirmButton = FindObjectOfType<ConfirmButton>();
            endGamePanel.gameObject.SetActive(false);
            cancelButton.Active = false;
            confirmButton.Active = false;
            barrelButton.Active = false;
            endTurnButton.Active = false;
            takeHitButton.Active = false;
            dieButton.Active = false;
            passButton.Active = false;
        }

        public override void EnableClick(bool value)
        {
            base.EnableClick(value);
            handHidden.SetActive(!value);
            if (!value) EnableClickHand(false);
            SetBackgroundColor(value ? Color.magenta : idle);
        }

        public void EnableClickHand(bool value)
        {
            handHidden.EnableClick(value);
        }

        public void EnableClickProperties(bool value, bool weapon)
        {
            if (weapon || !value) weaponCard.EnableClick(value);
            propertyCards.SetClickable(value);
        }

        public void SetTurn(bool value)
        {
            turn.SetActive(value);
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

        public void SetCharacter(string character)
        {
            this.character.text = character;
        }

        public void UpdateHP(int hp)
        {
            this.hp.text = hp.ToString();
        }

        public void AddHandCard()
        {
            HiddenCards += 1;
        }

        public void RemoveHandCard()
        {
            HiddenCards -= 1;
        }

        public void AddHandCard(int index, CardStruct cs)
        {
            handCards.AddCard(index, cs, this);
        }

        public void EquipProperty(int index, CardStruct cs)
        {
            propertyCards.AddCard(index, cs, this);
        }

        public void RemoveHandCard(int index)
        {
            handCards.RemoveCard(index);
        }

        public void RemoveProperty(int index)
        {
            propertyCards.RemoveCard(index);
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
                    handHidden.Droppable = value;
                }
                else
                {
                    handCards.SetDroppable(value);
                }
            }
            if (!value || weapon) weaponCard.Droppable = value;
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

        public void EnableBarrelButton(bool value)
        {
            barrelButton.Active = value;
        }

        public void EnablePassButton(bool value)
        {
            passButton.Active = value;
        }

        public override void Click()
        {
            PlayerController.LocalPlayer.PhaseOneDecision(Decision.Player, PlayerIndex);
        }

        public void EnableCancelButton(bool enable)
        {
            cancelButton.Active = enable;
        }

        public void EnableConfirmButton(bool enable)
        {
            confirmButton.Active = enable;
        }

        public void EnablePlayerSkill(bool value)
        {
            skill.SetActive(value);
        }

        public void SetPlayerSkillStatus(bool value)
        {
            skill.SetStatus(value);
        }

        public void SetTurnCountdown(float time)
        {
            turnCountdown.SetCountdown(time);
        }

        public void SetTurnTimeSpent(float time)
        {
            turnCountdown.SetTimeSpent(time);
        }

        public void SetResponseCountdown(float time)
        {
            responseCountdown.SetCountdown(time);
        }

        public void SetResponseTimeSpent(float time)
        {
            responseCountdown.SetTimeSpent(time);
        }
    }
}