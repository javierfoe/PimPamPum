using Mirror;

namespace PimPamPum
{
    public class ActionsController : NetworkBehaviour
    {
        private readonly SyncListCard cards = new SyncListCard();

        [SyncVar(hook = nameof(SetTakeHit))] private bool takeHit;
        [SyncVar(hook = nameof(SetConfirm))] private bool confirm;
        [SyncVar(hook = nameof(SetCancel))] private bool cancel;
        [SyncVar(hook = nameof(SetBarrel))] private bool barrel;
        [SyncVar(hook = nameof(SetDie))] private bool die;
        [SyncVar(hook = nameof(SetPass))] private bool pass;
        [SyncVar(hook = nameof(SetEndTurn))] private bool endTurn;
        [SyncVar(hook = nameof(SetSkill))] private bool playerSkill;
        [SyncVar(hook = nameof(EnableSkill))] private bool playerSkillEnable;

        private PlayerController playerController;

        public bool TakeHit { set { takeHit = value; } }
        public bool Confirm { set { confirm = value; } }
        public bool Cancel { set { cancel = value; } }
        public bool Barrel { set { barrel = value; } }
        public bool Die { set { die = value; } }
        public bool Pass { set { pass = value; } }
        public bool EndTurn { set { endTurn = value; } }
        public bool PlayerSkill { set { playerSkill = value; } }
        public bool PlayerSkillEnable { set { playerSkillEnable = value; } }

        public override void OnStartServer()
        {
            playerController = GetComponent<PlayerController>();
        }

        public override void OnStartLocalPlayer()
        {
            playerController = GetComponent<PlayerController>();
            cards.Callback += OnCardsUpdated;
        }

        public void AddCard(CardStruct card)
        {
            cards.Add(card);
        }

        public void SetCard(int index, bool value)
        {
            CardStruct card = cards[index];
            card.enabled = value;
            cards[index] = card;
        }

        public void RemoveCard(int index)
        {
            cards.RemoveAt(index);
        }

        private void OnCardsUpdated(SyncListCard.Operation op, int index, CardStruct card)
        {
            switch (op)
            {
                case SyncListCard.Operation.OP_ADD:
                    playerController.AddHandCard(index, card);
                    break;
                case SyncListCard.Operation.OP_REMOVEAT:
                    playerController.RemoveHandCard(index);
                    break;
                case SyncListCard.Operation.OP_SET:
                    playerController.EnableCard(index, card.enabled);
                    break;
            }
        }

        private void SetTakeHit(bool value)
        {
            playerController.EnableTakeHitButton(value);
        }

        private void SetConfirm(bool value)
        {
            playerController.EnableConfirmButton(value);
        }

        private void SetCancel(bool value)
        {
            playerController.EnableCancelButton(value);
        }

        private void SetBarrel(bool value)
        {
            playerController.EnableBarrelButton(value);
        }

        private void SetDie(bool value)
        {
            playerController.EnableDieButton(value);
        }

        private void SetPass(bool value)
        {
            playerController.EnablePassButton(value);
        }

        private void SetEndTurn(bool value)
        {
            playerController.EnableEndTurnButton(value);
        }

        private void SetSkill(bool value)
        {
            playerController.SetSkill(value);
        }

        private void EnableSkill(bool value)
        {
            playerController.EnableSkill(value);
        }
    }
}