using UnityEngine;

namespace Bang
{
    public interface IBoardView
    {
        void SetDeckSize(int cards);
        void SetDiscardTop(string name, ESuit suit, ERank rank, Color color);
    }

    public interface IDropView
    {
        void SetDroppable(ECardDropArea cda);
    }

    public interface ICardView : IDropView
    {
        void Playable(bool value);
        void SetIndex(int index);
        void SetRank(ERank rank);
        void SetSuit(ESuit suit);
        void SetName(string name, Color color);
        void Empty();
    }

    public interface IPlayerView : IDropView
    {
        void SetStealable(ECardDropArea cda);
        void SetWeaponStealable(ECardDropArea cda);
        void SetPlayerIndex(int index);
        void UpdateHP(int hp);
        void SetSheriff();
        void SetRole(ERole role);
        void EnableCard(int index, bool enable);
        void AddCard();
        void AddCard(int index, string name, ESuit suit, ERank rank, Color color);
        void EquipWeapon(string name, ESuit suit, ERank rank, Color color);
    }
}
