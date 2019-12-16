namespace PimPamPum
{
    public interface IPlayerView : IDropView, IClickView
    {
        int PlayerIndex { get; set; }
        void SetCharacter(string character);
        void SetPlayerName(string name);
        void SetTurn(bool value);
        void SetStealable(bool value, bool hand, bool weapon);
        void EnableClickHand(bool value);
        void EnableClickProperties(bool value, bool weapon);
        void SetLocalPlayer();
        void UpdateHP(int hp);
        void SetSheriff();
        void SetRole(Role role);
        void EnableEndTurnButton(bool enable);
        void EnableBarrelButton(bool enable);
        void EnableTakeHitButton(bool enable);
        void EnableDieButton(bool enable);
        void EnablePassButton(bool enable);
        void EnableCancelButton(bool enable);
        void EnableConfirmButton(bool enable);
        void EnableCard(int index, bool enable);
        void AddHandCard(int index, CardValues cs);
        void EquipProperty(int index, CardValues cs);
        void RemoveProperty(int index);
        void UpdateCards(int cards);
        void RemoveHandCard(int index);
        void EquipWeapon(CardValues cs);
        void EnablePlayerSkill(bool value);
        void SetPlayerSkillStatus(bool value);
        void Win();
        void Lose();
        void EnableTurn(bool value);
        void EnableResponse(bool value);
        void SetTurnCountdown(float time);
        void SetTurnTimeSpent(float time);
        void SetResponseCountdown(float time);
        void SetResponseTimeSpent(float time);
        void SetStatus(PlayerViewStatus status);
    }
}
