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
        void AddHandCard();
        void AddHandCard(int index, CardStruct cs);
        void EquipProperty(int index, CardStruct cs);
        void RemoveProperty(int index);
        void RemoveHandCard();
        void RemoveHandCard(int index);
        void EquipWeapon(CardStruct cs);
        void EnablePlayerSkill(bool value);
        void SetPlayerSkillStatus(bool value);
        void Win();
        void Lose();
    }
}
