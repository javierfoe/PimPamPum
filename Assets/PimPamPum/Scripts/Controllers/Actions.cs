using System.Collections.Generic;
using UnityEngine;

namespace PimPamPum
{
    public class Actions : MonoBehaviour
    {

        private readonly List<CardValues> cards = new List<CardValues>();
        private readonly List<PlayerViewStatus> players = new List<PlayerViewStatus>();

        public bool Thrash { get; set; }
        public bool TakeHit { get; set; }
        public bool Confirm { get; set; }
        public bool Cancel { get; set; }
        public bool Barrel { get; set; }
        public bool Die { get; set; }
        public bool Pass { get; set; }
        public bool EndTurn { get; set; }
        public bool PlayerSkill { get; set; }
        public bool PlayerSkillEnable { get; set; }

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

        private void OnPlayersUpdated(int index, PlayerViewStatus newValue)
        {
            GameController.SetPlayerView(index, newValue);
        }

        private void SetTakeHit(bool value)
        {
            PlayerView.LocalPlayer?.EnableTakeHitButton(value);
        }

        private void SetConfirm(bool value)
        {
            PlayerView.LocalPlayer?.EnableConfirmButton(value);
        }

        private void SetCancel(bool value)
        {
            PlayerView.LocalPlayer?.EnableCancelButton(value);
        }

        private void SetBarrel(bool value)
        {
            PlayerView.LocalPlayer?.EnableBarrelButton(value);
        }

        private void SetDie(bool value)
        {
            PlayerView.LocalPlayer?.EnableDieButton(value);
        }

        private void SetPass(bool value)
        {
            PlayerView.LocalPlayer?.EnablePassButton(value);
        }

        private void SetEndTurn(bool value)
        {
            PlayerView.LocalPlayer?.EnableEndTurnButton(value);
        }

        private void SetSkill(bool value)
        {
            PlayerView.LocalPlayer?.SetPlayerSkillStatus(value);
        }

        private void EnableSkill(bool value)
        {
            PlayerView.LocalPlayer?.EnablePlayerSkill(value);
        }

        private void SetThrash(bool value)
        {
            GameController.SetTargetableThrash(value);
        }
    }
}