using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PimPamPum
{

    public abstract class DecisionMaking : IEnumerator
    {

        private static DecisionMaking decisionMaking;

        public object Current { get; protected set; }

        protected DecisionMaking()
        {
            decisionMaking = this;
        }

        public static void MakeDecision(int card)
        {
            decisionMaking.MakeDecision(0, card, Decision.Pending);
        }

        protected virtual void MakeDecision(int player, int card, Decision decision) { }

        public abstract bool MoveNext();

        public void Reset() { }

    }

    public class Timer : DecisionMaking
    {
        private float time, maxTime;

        public bool TimeUp
        {
            get; private set;
        }

        public override bool MoveNext()
        {
            time += Time.deltaTime;
            bool timer = time < maxTime;
            TimeUp = !timer;
            return timer;
        }

        protected Timer(float maxTime)
        {
            time = 0;
            this.maxTime = maxTime;
        }

    }

    public class GeneralStoreTimer : Timer
    {

        private int cardAmount;
        protected NetworkConnection conn;

        public int Choice { get; private set; }
        public Card ChosenCard { get; private set; }
        public List<Card> NotChosenCards { get; private set; }
        public List<Card> Cards { get; protected set; }

        public override bool MoveNext()
        {
            bool res = base.MoveNext() && Choice < 0;
            if (!res && Choice < 0)
            {
                int random = UnityEngine.Random.Range(0, cardAmount);
                MakeDecision(random);
            }
            if (!res)
            {
                FinishedCoroutine();
            }
            return res;
        }

        protected GeneralStoreTimer(NetworkConnection conn, int cards, float maxTime) : base(maxTime)
        {
            this.conn = conn;
            Choice = -1;
            cardAmount = cards;
        }

        public GeneralStoreTimer(NetworkConnection conn, List<Card> cards, float maxTime) : this(conn, cards.Count, maxTime)
        {
            Cards = cards;
            GameController.Instance.EnableGeneralStoreCards(conn, true);
        }

        protected override void MakeDecision(int player, int card, Decision decision)
        {
            Choice = card;
            NotChosenCards = new List<Card>(Cards);
            ChosenCard = Cards[card];
            NotChosenCards.RemoveAt(card);
        }

        protected virtual void FinishedCoroutine()
        {
            GameController.Instance.EnableGeneralStoreCards(conn, false);
        }

    }

    public class ChooseCardTimer : GeneralStoreTimer
    {

        public ChooseCardTimer(NetworkConnection conn, int cards, float maxTime) : base(conn, cards, maxTime)
        {
            Cards = GameController.Instance.DrawChooseCards(cards, conn);
        }

        protected override void FinishedCoroutine()
        {
            GameController.Instance.RemoveSelectableCardsAndDisable(conn);
        }

    }

    public class DecisionTimer : Timer
    {

        private Decision decision;

        public Decision Decision
        {
            get; private set;
        }

        public bool DecisionMade
        {
            get; private set;
        }

        public override bool MoveNext()
        {
            return base.MoveNext() && !DecisionMade;
        }

        protected DecisionTimer(float maxTime, Decision decision) : base(maxTime)
        {
            this.decision = decision;
            Decision = decision;
        }

        protected override void MakeDecision(int player, int card, Decision decision)
        {
            Decision = decision;
            DecisionMade = decision != this.decision;
        }

    }

    public class PendingTimer : DecisionTimer
    {

        public PendingTimer(float maxTime) : base(maxTime, Decision.Pending) { }

    }

    public class DyingTimer : DecisionTimer
    {

        private Func<bool> dying;

        public override bool MoveNext()
        {
            return dying();
        }

        public DyingTimer(float maxTime, PlayerController pc) : base(maxTime, Decision.Die)
        {
            dying = () => base.MoveNext() && pc.IsDying;
        }

    }
}