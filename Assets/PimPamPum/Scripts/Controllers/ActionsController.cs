using Mirror;

namespace PimPamPum
{
    public class ActionsController : NetworkBehaviour
    {
        private readonly SyncListCard _cards = new SyncListCard();

        [field: SyncVar(hook = nameof(SetTakeHit))] public bool TakeHit { get; set; }
        [field: SyncVar(hook = nameof(SetConfirm))] public bool Confirm { get; set; }
        [field: SyncVar(hook = nameof(SetCancel))] public bool Cancel { get; set; }
        [field: SyncVar(hook = nameof(SetBarrel))] public bool Barrel { get; set; }
        [field: SyncVar(hook = nameof(SetDie))] public bool Die { get; set; }
        [field: SyncVar(hook = nameof(SetPass))] public bool Pass { get; set; }
        [field: SyncVar(hook = nameof(SetEndTurn))] public bool EndTurn { get; set; }
        [field: SyncVar(hook = nameof(SetSkill))] public bool PlayerSkill { get; set; }
        [field: SyncVar(hook = nameof(EnableSkill))] public bool PlayerSkillEnable { get; set; }
        
        public override void OnStartLocalPlayer()
        {
            _cards.Callback += OnCardsUpdated;
        }

        public void AddCard(CardStruct card)
        {
            _cards.Add(card);
        }

        public void SetCard(int index, bool value)
        {
            CardStruct card = _cards[index];
            card.enabled = value;
            _cards[index] = card;
        }

        public void RemoveCard(int index)
        {
            _cards.RemoveAt(index);
        }

        private void OnCardsUpdated(SyncListCard.Operation op, int index, CardStruct oldValue, CardStruct newValue)
        {
            switch (op)
            {
                case SyncListCard.Operation.OP_ADD:
                    PlayerView.LocalPlayer.AddHandCard(index, newValue);
                    break;
                case SyncListCard.Operation.OP_REMOVEAT:
                    PlayerView.LocalPlayer.RemoveHandCard(index);
                    break;
                case SyncListCard.Operation.OP_SET:
                    PlayerView.LocalPlayer.EnableCard(index, newValue.enabled);
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