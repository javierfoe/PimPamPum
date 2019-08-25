namespace PimPamPum
{
    public interface IPlayerView : ICardHolderView, ISelectView
    {
        void SetCharacter(string character);
        void SetPlayerName(string name);
        void SetTurn(bool value);
        void SetStealable(bool value, bool hand, bool weapon);
        void SetPlayerIndex(int index);
        void SetLocalPlayer();
        int GetPlayerIndex();
        void UpdateHP(int hp);
        void SetSheriff();
        void SetRole(Role role);
        void SetTextTakeHitButton(string text);
        void EnableEndTurnButton(bool enable);
        void EnableBarrelButton(bool enable);
        void EnableTakeHitButton(bool enable);
        void EnableDieButton(bool enable);
        void EnablePassButton(bool enable);
        void EnableCard(int index, bool enable);
        void AddCard();
        void EquipProperty(int index, CardStruct cs);
        void RemoveProperty(int index);
        void RemoveCard();
        void EquipWeapon(CardStruct cs);
        void Win();
        void Lose();
    }
}
