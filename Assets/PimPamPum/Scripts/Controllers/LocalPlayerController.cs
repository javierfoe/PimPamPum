using Mirror;

namespace PimPamPum
{
    public class LocalPlayerController : NetworkBehaviour
    {

        private readonly SyncListCard cards = new SyncListCard();
        private readonly SyncListPlayer players = new SyncListPlayer();

        [field: SyncVar(hook = nameof(SetThrash))] public bool Thrash { get; set; }
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
            cards.Callback += OnCardsUpdated;
            players.Callback += OnPlayersUpdated;
        }

        public void SetPlayerStatusArray(int number)
        {
            for(int i = 0; i < number; i++)
            {
                players.Add(new PlayerViewStatus());
            }
        }

        public void SetPlayersStatus(PlayerViewStatus[] status)
        {
            for(int i = 0; i < status.Length; i++)
            {
                players[i] = status[i];
            }
        }

        public void AddCard(CardValues card)
        {
            cards.Add(card);
        }

        public void SetCard(int index, bool value)
        {
            CardValues card = cards[index];
            card.enabled = value;
            cards[index] = card;
        }

        public void RemoveCard(int index)
        {
            cards.RemoveAt(index);
        }

        private void OnCardsUpdated(SyncListCard.Operation op, int index, CardValues oldValue, CardValues newValue)
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

        private void OnPlayersUpdated(SyncListPlayer.Operation op, int index, PlayerViewStatus oldValue, PlayerViewStatus newValue)
        {
            if (op != SyncListPlayer.Operation.OP_SET) return;
            GameController.Instance.SetPlayerView(index, newValue);
        }

        private void SetTakeHit(bool value)
        {
            if (!isLocalPlayer) return;
            PlayerView.LocalPlayer?.EnableTakeHitButton(value);
        }

        private void SetConfirm(bool value)
        {
            if (!isLocalPlayer) return;
            PlayerView.LocalPlayer?.EnableConfirmButton(value);
        }

        private void SetCancel(bool value)
        {
            if (!isLocalPlayer) return;
            PlayerView.LocalPlayer?.EnableCancelButton(value);
        }

        private void SetBarrel(bool value)
        {
            if (!isLocalPlayer) return;
            PlayerView.LocalPlayer?.EnableBarrelButton(value);
        }

        private void SetDie(bool value)
        {
            if (!isLocalPlayer) return;
            PlayerView.LocalPlayer?.EnableDieButton(value);
        }

        private void SetPass(bool value)
        {
            if (!isLocalPlayer) return;
            PlayerView.LocalPlayer?.EnablePassButton(value);
        }

        private void SetEndTurn(bool value)
        {
            if (!isLocalPlayer) return;
            PlayerView.LocalPlayer?.EnableEndTurnButton(value);
        }

        private void SetSkill(bool value)
        {
            if (!isLocalPlayer) return;
            PlayerView.LocalPlayer?.SetPlayerSkillStatus(value);
        }

        private void EnableSkill(bool value)
        {
            if (!isLocalPlayer) return;
            PlayerView.LocalPlayer?.EnablePlayerSkill(value);
        }

        private void SetThrash(bool value)
        {
            if (!isLocalPlayer) return;
            GameController.Instance.SetTargetableThrash(value);
        }
    }
}