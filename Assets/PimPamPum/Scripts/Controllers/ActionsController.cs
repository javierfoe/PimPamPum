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

        public bool TakeHit { set { takeHit = value; } }
        public bool Confirm { set { confirm = value; } }
        public bool Cancel { set { cancel = value; } }
        public bool Barrel { set { barrel = value; } }
        public bool Die { set { die = value; } }
        public bool Pass { set { pass = value; } }
        public bool EndTurn { set { endTurn = value; } }
        public bool PlayerSkill { set { playerSkill = value; } }
        public bool PlayerSkillEnable { set { playerSkillEnable = value; } }

        public override void OnStartLocalPlayer()
        {
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
                    PlayerView.LocalPlayer.AddHandCard(index, card);
                    break;
                case SyncListCard.Operation.OP_REMOVEAT:
                    PlayerView.LocalPlayer.RemoveHandCard(index);
                    break;
                case SyncListCard.Operation.OP_SET:
                    PlayerView.LocalPlayer.EnableCard(index, card.enabled);
                    break;
            }
        }

        private void SetTakeHit(bool value)
        {
            if (!isLocalPlayer) return;
            PlayerView.LocalPlayer.EnableTakeHitButton(value);
        }

        private void SetConfirm(bool value)
        {
            if (!isLocalPlayer) return;
            PlayerView.LocalPlayer.EnableConfirmButton(value);
        }

        private void SetCancel(bool value)
        {
            if (!isLocalPlayer) return;
            PlayerView.LocalPlayer.EnableCancelButton(value);
        }

        private void SetBarrel(bool value)
        {
            if (!isLocalPlayer) return;
            PlayerView.LocalPlayer.EnableBarrelButton(value);
        }

        private void SetDie(bool value)
        {
            if (!isLocalPlayer) return;
            PlayerView.LocalPlayer.EnableDieButton(value);
        }

        private void SetPass(bool value)
        {
            if (!isLocalPlayer) return;
            PlayerView.LocalPlayer.EnablePassButton(value);
        }

        private void SetEndTurn(bool value)
        {
            if (!isLocalPlayer) return;
            PlayerView.LocalPlayer.EnableEndTurnButton(value);
        }

        private void SetSkill(bool value)
        {
            if (!isLocalPlayer) return;
            PlayerView.LocalPlayer.SetPlayerSkillStatus(value);
        }

        private void EnableSkill(bool value)
        {
            if (!isLocalPlayer) return;
            PlayerView.LocalPlayer.EnablePlayerSkill(value);
        }
    }
}