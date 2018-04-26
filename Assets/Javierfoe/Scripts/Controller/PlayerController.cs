using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Bang
{
    public class PlayerController : NetworkBehaviour
    {

        [SyncVar] private int playerNum;

        public static PlayerController LocalPlayer { get; private set; }
        private static Colt45 colt45 = new Colt45();

        private int bangsUsed;
        private int hp;
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
            LocalPlayer = this;
        }

        public void SetRole(ERole role)
        {
            this.Role = role;
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

        public void BangBeginCardDrag()
        {
            GameController.Instance.CmdTargetPlayersRange(playerNum, weapon.Range);
        }

        public void JailBeginCardDrag()
        {
            GameController.Instance.CmdTargetAllButSheriff(playerNum);
        }

        public void CatBalouBeginCardDrag()
        {
            GameController.Instance.CmdTargetAllCards(playerNum);
        }

        public void PanicBeginCardDrag()
        {
            GameController.Instance.CmdTargetAllRangeCards(playerNum, 1);
        }

        public void TargetOthers()
        {
            GameController.Instance.CmdTargetOthers(playerNum);
        }

        public void Bang()
        {
            bangsUsed++;
        }

        private void EquipWeapon(Weapon weapon)
        {
            RpcEquipWeapon(weapon.ToString(), weapon.Suit, weapon.Rank, weapon.Color);
        }

        public void StopTargeting()
        {
            GameController.Instance.CmdStopTargeting(playerNum);
        }

        [Command]
        public void CmdBeginCardDrag(int index)
        {
            hand[index].BeginCardDrag(this);
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
        private void RpcSheriff()
        {
            PlayerView.SetSheriff();
        }

        [TargetRpc]
        public void TargetStopTargeting(NetworkConnection conn)
        {
            ECardDropArea cda = ECardDropArea.NULL;
            PlayerView.SetDroppable(cda);
            PlayerView.SetStealable(cda);
        }

        [TargetRpc]
        public void TargetSetTargetable(NetworkConnection conn, ECardDropArea cda)
        {
            PlayerView.SetDroppable(cda);
        }

        [TargetRpc]
        public void TargetSetStealable(NetworkConnection conn, ECardDropArea cda)
        {
            PlayerView.SetStealable(cda);
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

    }
}