using UnityEngine;

namespace PimPamPum
{
    public interface IBoardView : IDropView
    {
        void EnableGeneralStore(bool value);
        void EnableGeneralStoreCards(bool value);
        void AddGeneralStoreCard(int index, CardStruct cs);
        void RemoveGeneralStoreCard(int index);
        void RemoveAllGeneralStoreCards();
        void SetDeckSize(int cards);
        void SetDiscardTop(CardStruct cs);
        void EmptyDiscardStack();
    }

    public interface IDropView
    {
        GameObject GameObject();
        void SetTargetable(bool value);
        bool GetDroppable();
        void SetDroppable(bool value);
        Drop GetDropEnum();
        int GetDropIndex();
    }

    public interface ICardView : IDropView
    {
        void Playable(bool value);
        void SetIndex(int index);
        void SetCard(CardStruct cs);
        void Empty();
    }

    public interface IGeneralStoreCardView : ICardView
    {
        void Enable(bool value);
    }

    public interface IPlayerView : IDropView
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
        void AddCard(int index, CardStruct cs);
        void EquipProperty(int index, CardStruct cs);
        void RemoveProperty(int index);
        void RemoveCard();
        void RemoveCard(int index);
        void EquipWeapon(CardStruct cs);
        void Win();
        void Lose();
    }
}
