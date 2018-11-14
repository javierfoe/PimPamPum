﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Bang
{
    public class PlayerController : NetworkBehaviour
    {

        [SyncVar] private int playerNum;

        public static PlayerController LocalPlayer { get; private set; }
        private static Colt45 colt45 = new Colt45();

        private Card draggedCard;
        private int bangsUsed;
        private int hp;
        private EndTurnButton endTurn;
        private Weapon weapon;
        private GameController gameController;
        private List<Card> hand, properties;
        private IPlayerView playerView;

        public IPlayerView PlayerView
        {
            private get
            {
                return playerView;
            }
            set
            {
                playerView = value;
                playerView.SetPlayerIndex(playerNum);
            }
        }

        private int HP
        {
            get { return hp; }
            set
            {
                hp = value;
                RpcUpdateHP(hp);
            }
        }

        public int PlayerNumber
        {
            get { return playerNum; }
            set { playerNum = value; }
        }

        public ERole Role
        {
            private set; get;
        }

        public Weapon Weapon
        {
            get { return weapon; }
            set
            {
                weapon = value;
                EquipWeapon(value);
            }
        }

        public override void OnStartServer()
        {
            gameController = GameController.Instance;
            hand = new List<Card>();
            properties = new List<Card>();
        }

        public override void OnStartLocalPlayer()
        {
            endTurn = FindObjectOfType<EndTurnButton>();
            endTurn.Active = false;
            LocalPlayer = this;
        }

        public void SetRole(ERole role)
        {
            Role = role;
            if (role == ERole.SHERIFF)
            {
                HP = 5;
                RpcSheriff();
            }
            else
            {
                HP = 4;
                TargetSetRole(this.connectionToClient, role);
            }
            Weapon = colt45;
            DrawInitialCards();
        }

        public void Draw(int amount)
        {
            List<Card> cards = gameController.DrawCards(amount);
            foreach (Card card in cards)
            {
                AddCard(card);
            }
        }

        private void DrawInitialCards()
        {
            Draw(hp);
        }

        private void AddCard(Card c)
        {
            hand.Add(c);
            TargetAddCard(connectionToClient, hand.Count - 1, c.ToString(), c.Suit, c.Rank, c.Color);
            RpcAddCard();
        }

        public void StartTurn()
        {
            Draw(2);
            bangsUsed = 0;
            EnableCards(true);
            TargetEndTurnButton(connectionToClient);
        }

        public void EndTurn()
        {
            endTurn.Active = false;
            CmdEndTurn();
        }

        private void DiscardEndTurn(bool value)
        {
            int length = hand.Count;
            for (int i = 0; i < length; i++)
            {
                TargetEnableDiscardCard(connectionToClient, i, value);
            }
        }

        private void EnableCards(bool value)
        {
            int length = hand.Count;
            for (int i = 0; i < length; i++)
            {
                TargetEnableCard(connectionToClient, i, !(hand[i] is Missed) && value);
            }
        }

        public void BeginCardDrag(int index)
        {
            CmdBeginCardDrag(index);
        }

        public void PlayCard(int player, int drop)
        {
            CmdPlayCard(player, drop);
        }

        public void DiscardCard(int index)
        {
            CmdDiscardCard(index);
        }

        public void Bang()
        {
            bangsUsed++;
        }

        private void EquipWeapon(Weapon weapon)
        {
            RpcEquipWeapon(weapon.ToString(), weapon.Suit, weapon.Rank, weapon.Color);
        }

        public void SetStealable(NetworkConnection conn, bool value)
        {
            TargetSetStealable(conn, value, weapon != colt45);
        }

        public void EndCardDrag()
        {
            draggedCard = null;
            GameController.Instance.StopTargeting();
        }

        public void BangBeginCardDrag()
        {
            GameController.Instance.TargetPlayersRange(playerNum, weapon.Range);
        }

        public void JailBeginCardDrag()
        {
            GameController.Instance.TargetAllButSheriff(playerNum);
        }

        public void CatBalouBeginCardDrag()
        {
            GameController.Instance.TargetAllCards(playerNum);
        }

        public void PanicBeginCardDrag()
        {
            GameController.Instance.TargetAllRangeCards(playerNum, 1);
        }

        public void TargetOthers()
        {
            GameController.Instance.TargetOthers(playerNum);
        }

        public void SelfTargetCard()
        {
            GameController.Instance.TargetSelf(playerNum);
        }

        public void StopTargeting()
        {
            PlayerView.SetDroppable(false);
            PlayerView.SetStealable(false, true);
        }

        [Command]
        public void CmdPlayCard(int player, int drop)
        {
            draggedCard.PlayCard(player, drop);
        }

        [Command]
        public void CmdSelfTargetCard()
        {
            SelfTargetCard();
        }

        [Command]
        public void CmdBeginCardDrag(int index)
        {
            draggedCard = hand[index];
            draggedCard.BeginCardDrag(this);
        }

        [Command]
        public void CmdEndTurn()
        {
            EnableCards(false);
            if (hand.Count < hp + 1)
            {
                GameController.Instance.EndTurn();
            }
            else
            {
                DiscardEndTurn(true);
            }
        }

        [Command]
        public void CmdDiscardCard(int index)
        {
            Card card = hand[index];
            hand.RemoveAt(index);
            GameController.Instance.DiscardCard(card);
            TargetRemoveCard(connectionToClient, index);
            RpcRemoveCard();
            if (hand.Count < hp + 1)
            {
                GameController.Instance.EndTurn();
                DiscardEndTurn(false);
            }
        }

        [ClientRpc]
        private void RpcEquipWeapon(string name, ESuit suit, ERank rank, Color color)
        {
            PlayerView.EquipWeapon(name, suit, rank, color);
        }

        [ClientRpc]
        private void RpcUpdateHP(int hp)
        {
            PlayerView.UpdateHP(hp);
        }

        [ClientRpc]
        private void RpcAddCard()
        {
            if (isLocalPlayer) return;
            PlayerView.AddCard();
        }

        [ClientRpc]
        private void RpcRemoveCard()
        {
            if (isLocalPlayer) return;
            PlayerView.RemoveCard();
        }

        [ClientRpc]
        private void RpcSheriff()
        {
            PlayerView.SetSheriff();
        }

        [TargetRpc]
        public void TargetSetTargetable(NetworkConnection conn, bool value)
        {
            PlayerView.SetDroppable(value);
        }

        [TargetRpc]
        public void TargetSetStealable(NetworkConnection conn, bool value, bool weapon)
        {
            PlayerView.SetStealable(value, weapon);
        }

        [TargetRpc]
        private void TargetEnableDiscardCard(NetworkConnection conn, int card, bool value)
        {
            PlayerView.EnableDiscardCard(card, value);
        }

        [TargetRpc]
        private void TargetEnableCard(NetworkConnection conn, int card, bool value)
        {
            PlayerView.EnableCard(card, value);
        }

        [TargetRpc]
        private void TargetAddCard(NetworkConnection conn, int index, string name, ESuit suit, ERank rank, Color color)
        {
            PlayerView.AddCard(index, name, suit, rank, color);
        }

        [TargetRpc]
        private void TargetRemoveCard(NetworkConnection conn, int index)
        {
            PlayerView.RemoveCard(index);
        }

        [TargetRpc]
        private void TargetSetRole(NetworkConnection conn, ERole role)
        {
            PlayerView.SetRole(role);
        }

        [TargetRpc]
        public void TargetSetup(NetworkConnection conn, int playerNumber)
        {
            GameController gameController = GameController.Instance;
            IPlayerView ipv = null;
            if (isLocalPlayer)
            {
                ipv = gameController.GetPlayerView(0);
            }
            else
            {
                ipv = gameController.GetPlayerView(playerNumber, this.PlayerNumber);
            }
            PlayerView = ipv;
        }

        [TargetRpc]
        public void TargetEndTurnButton(NetworkConnection conn)
        {
            endTurn.Active = true;
        }

    }
}