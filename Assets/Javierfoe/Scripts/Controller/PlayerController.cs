using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Bang
{
    public class PlayerController : NetworkBehaviour
    {

        [SyncVar] private int playerNum;

        public static PlayerController LocalPlayer { get; private set; }
        private static GameController GameController { get; set; }
        private static Colt45 colt45 = new Colt45();

        private delegate void OnStartTurn();
        private OnStartTurn onStartTurn;
        private int draggedCard, bangsUsed, hp, maxHp;
        private EndTurnButton endTurnButton;
        private Weapon weapon;
        private List<Card> hand, properties;
        private IPlayerView playerView;
        private bool endTurn;

        private bool EndTurnOnStart
        {
            get
            {
                return endTurn || IsDead;
            }
            set
            {
                endTurn = value;
            }
        }

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

        public bool IsDead
        {
            get { return HP < 1; }
        }

        private int MaxHP
        {
            get { return maxHp; }
            set
            {
                maxHp = value;
                HP = value;
            }
        }

        public int PlayerNumber
        {
            get { return playerNum; }
            set { playerNum = value; }
        }

        public int Scope
        {
            get; private set;
        }

        public int RangeModifier
        {
            get; private set;
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
                RpcEquipWeapon(weapon.ToString(), weapon.Suit, weapon.Rank, weapon.Color);
            }
        }

        public override void OnStartServer()
        {
            hand = new List<Card>();
            properties = new List<Card>();
        }

        public override void OnStartLocalPlayer()
        {
            endTurnButton = FindObjectOfType<EndTurnButton>();
            endTurnButton.Active = false;
            LocalPlayer = this;
        }

        public override void OnStartClient()
        {
            if (!GameController) GameController = FindObjectOfType<GameController>();
        }

        public virtual void SetRole(ERole role)
        {
            Role = role;
            if (role == ERole.SHERIFF)
            {
                MaxHP = 5;
                RpcSheriff();
            }
            else
            {
                MaxHP = 4;
                TargetSetRole(connectionToClient, role);
            }
            Weapon = colt45;
            DrawInitialCards();
        }

        public void Draw(int amount)
        {
            List<Card> cards = GameController.DrawCards(amount);
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

        public Card UnequipDraggedCard()
        {
            Card c = hand[draggedCard];
            RemoveCardFromHand(draggedCard);
            return c;
        }

        private void RemoveCardFromHand(int index)
        {
            hand.RemoveAt(index);
            TargetRemoveCard(connectionToClient, index);
            RpcRemoveCard();
        }

        public void EquipProperty(Property c)
        {
            properties.Add(c);
            RpcEquipProperty(properties.Count - 1, c.ToString(), c.Suit, c.Rank, c.Color);
        }

        public void EquipJail()
        {
            onStartTurn += JailCheck;
        }

        public void UnequipJail()
        {
            onStartTurn -= JailCheck;
        }

        public void EquipDynamite()
        {
            onStartTurn += DynamiteCheck;
        }

        public void UnequipDynamite()
        {
            onStartTurn -= DynamiteCheck;
        }

        public void EquipScope()
        {
            Scope++;
        }

        public void UnequipScope()
        {
            Scope--;
        }

        public void EquipMustang()
        {
            RangeModifier++;
        }

        public void UnequipMustang()
        {
            RangeModifier--;
        }

        public void StartTurn()
        {
            EndTurnOnStart = false;
            if (onStartTurn != null) onStartTurn();
            if (EndTurnOnStart)
            {
                TargetEndTurn(connectionToClient);
                return;
            }
            Draw(2);
            bangsUsed = 0;
            EnableCards(true);
            TargetEndTurnButton(connectionToClient);
        }

        public void DynamiteCheck()
        {
            Card c = GameController.DrawDiscardCard();
            int index;
            Dynamite d = FindProperty<Dynamite>(out index);
            UnequipProperty(index);
            if (d.CheckCondition(c))
            {
                Hit(3);
                if (!IsDead)
                {
                    GameController.DiscardCard(d);
                }
            }
            else
            {
                GameController.PassDynamite(playerNum, d);
            }
        }

        public void JailCheck()
        {
            Card c = GameController.DrawDiscardCard();
            int index;
            Jail j = FindProperty<Jail>(out index);
            endTurn = !j.CheckCondition(c);
            UnequipProperty(index);
            GameController.DiscardCard(j);
        }

        private T FindProperty<T>(out int index) where T : Card
        {
            bool found = false;
            T res = null;
            Card c;
            index = -1;
            for(int i = 0; i < properties.Count && !found; i++)
            {
                c = properties[i];
                found = c is T;
                index = found ? i : index;
                res = found ? (T)c : res;
            }
            return res;
        }

        public void EndTurn()
        {
            endTurnButton.Active = false;
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

        public void DiscardCardFromHand(int index)
        {
            CmdDiscardCardFromHand(index);
        }

        public void DiscardCardEndTurn(int index)
        {
            CmdDiscardCardFromHandEndTurn(index);
        }

        public void Bang()
        {
            bangsUsed++;
        }

        public void EquipWeapon(Weapon weapon)
        {
            if (Weapon != colt45)
            {
                GameController.DiscardCard(Weapon);
            }
            Weapon = weapon;
        }

        public bool HasProperty<T>() where T : Property
        {
            return Has<T>(properties);
        }

        public bool HasHand<T>() where T : Card
        {
            return Has<T>(hand);
        }

        private bool Has<T>(List<Card> list) where T : Card
        {
            bool res = false;
            int length = list.Count;
            for (int i = 0; i < length && !res; i++)
            {
                res = list[i] is T;
            }
            return res;
        }

        public void SetStealable(NetworkConnection conn, bool value)
        {
            TargetSetStealable(conn, value, weapon != colt45);
        }

        public void EndCardDrag()
        {
            draggedCard = -1;
            GameController.StopTargeting();
        }

        public void BangBeginCardDrag()
        {
            GameController.TargetPlayersRange(playerNum, weapon.Range + Scope);
        }

        public void JailBeginCardDrag()
        {
            GameController.TargetPrison(playerNum);
        }

        public void CatBalouBeginCardDrag()
        {
            GameController.TargetAllCards(playerNum);
        }

        public void PanicBeginCardDrag()
        {
            GameController.TargetAllRangeCards(playerNum, 1 + Scope);
        }

        public void TargetOthers()
        {
            GameController.TargetOthers(playerNum);
        }

        public void SelfTargetCard()
        {
            GameController.TargetSelf(playerNum);
        }

        public void SelfTargetPropertyCard<T>() where T : Property
        {
            GameController.TargetSelfProperty<T>(playerNum);
        }

        public void StopTargeting()
        {
            PlayerView.SetDroppable(false);
            PlayerView.SetStealable(false, true);
        }

        public void Hit(int amount)
        {
            HP -= amount;
            for (int i = 0; i < amount; i++) Hit();
            if (IsDead) Die();
        }

        public virtual void Hit() { }

        public virtual void Die()
        {
            for(int i = hand.Count - 1; i > -1; i--)
            {
                DiscardCardFromHand(i);
            }
            for (int i = properties.Count - 1; i > -1; i--)
            {
                DiscardProperty(i);
            }
            DiscardWeapon();
        }

        public virtual void Heal()
        {
            if (HP < MaxHP) HP++;
        }

        public void DiscardCardUsed()
        {
            DiscardCardFromHand(draggedCard);
        }

        public void FinishCardUsed()
        {
            EnableCards(true);
        }

        public void DiscardCardFromHandCmd(int index)
        {
            Card card = UnequipHandCard(index);
            GameController.DiscardCard(card);
        }

        public void DiscardWeapon()
        {
            if (Weapon == colt45) return;
            Weapon weapon = Weapon;
            UnequipWeapon();
            GameController.DiscardCard(weapon);
        }

        public void UnequipWeapon()
        {
            Weapon.UnequipProperty(this);
            Weapon = colt45;
        }

        public Card UnequipHandCard(int index)
        {
            Card card = hand[index];
            hand.RemoveAt(index);
            TargetRemoveCard(connectionToClient, index);
            RpcRemoveCard();
            return card;
        }

        public Card UnequipProperty(int index)
        {
            Property card = (Property)properties[index];
            properties.RemoveAt(index);
            card.UnequipProperty(this);
            RpcRemoveProperty(index);
            return card;
        }

        public void DiscardProperty(int index)
        {
            Card card = UnequipProperty(index);
            DiscardProperty(card);
        }

        public void DiscardProperty(Card card)
        {
            GameController.DiscardCard(card);
        }

        [Command]
        public void CmdPlayCard(int player, int drop)
        {
            hand[draggedCard].PlayCard(this, player, drop);
        }

        [Command]
        public void CmdSelfTargetCard()
        {
            SelfTargetCard();
        }

        [Command]
        public void CmdBeginCardDrag(int index)
        {
            draggedCard = index;
            hand[draggedCard].BeginCardDrag(this);
        }

        [Command]
        public void CmdEndTurn()
        {
            EnableCards(false);
            if (hand.Count < hp + 1)
            {
                GameController.EndTurn();
            }
            else
            {
                DiscardEndTurn(true);
            }
        }

        [Command]
        public void CmdDiscardCardFromHand(int index)
        {
            DiscardCardFromHandCmd(index);
        }

        [Command]
        public void CmdDiscardCardFromHandEndTurn(int index)
        {
            DiscardCardFromHandCmd(index);
            if (hand.Count < hp + 1)
            {
                GameController.EndTurn();
                DiscardEndTurn(false);
            }
        }

        [ClientRpc]
        private void RpcEquipWeapon(string name, ESuit suit, ERank rank, Color color)
        {
            PlayerView.EquipWeapon(name, suit, rank, color);
        }

        [ClientRpc]
        private void RpcEquipProperty(int index, string name, ESuit suit, ERank rank, Color color)
        {
            PlayerView.EquipProperty(index, name, suit, rank, color);
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
        private void RpcRemoveProperty(int index)
        {
            PlayerView.RemoveProperty(index);
        }

        [ClientRpc]
        private void RpcSheriff()
        {
            PlayerView.SetSheriff();
        }

        [TargetRpc]
        public void TargetEndTurn(NetworkConnection conn)
        {
            EndTurn();
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
            IPlayerView ipv = null;
            if (isLocalPlayer)
            {
                ipv = GameController.GetPlayerView(0);
            }
            else
            {
                ipv = GameController.GetPlayerView(playerNumber, this.PlayerNumber);
            }
            PlayerView = ipv;
        }

        [TargetRpc]
        public void TargetEndTurnButton(NetworkConnection conn)
        {
            endTurnButton.Active = true;
        }

    }
}