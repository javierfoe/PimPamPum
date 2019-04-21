using UnityEngine;

namespace PimPamPum
{
    public interface IBoardView : IDropView
    {
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

    public interface ISelectView : ICardView
    {
        void Enable(bool value);
    }

    public interface ICardHolderView
    {
        void AddCard(int index, CardStruct cs);
        void RemoveCard(int index);
    }

    public interface ISelectCardListView : ICardHolderView
    {
        void Enable(bool value);
        void EnableCards(bool value);
        void RemoveAllCards();
    }

    public interface IPlayerView : ICardHolderView, IDropView
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
