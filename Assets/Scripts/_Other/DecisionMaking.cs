using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PimPamPum
{

    public abstract class DecisionMaking : Enumerator
    {

        public static DecisionMaking CurrentTimer;

        protected DecisionMaking()
        {
            CurrentTimer = this;
        }

        public virtual void MakeDecision(int card) { }
        public virtual void MakeDecision(Decision decision, Card card = null) { }

    }

    public class Timer : DecisionMaking
    {
        private float time;

        public bool TimeUp
        {
            get; private set;
        }

        public override bool MoveNext()
        {
            time += Time.deltaTime;
            bool timer = time < GameController.MaxTime;
            TimeUp = !timer;
            return timer;
        }

        protected Timer()
        {
            time = 0;
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

        protected GeneralStoreTimer(NetworkConnection conn, int cards) : base()
        {
            this.conn = conn;
            Choice = -1;
            cardAmount = cards;
        }

        public GeneralStoreTimer(NetworkConnection conn, List<Card> cards) : this(conn, cards.Count)
        {
            Cards = cards;
            GameController.Instance.EnableGeneralStoreCards(conn, true);
        }

        public override void MakeDecision(int card)
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

        public ChooseCardTimer(NetworkConnection conn, int cards) : base(conn, cards)
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
        private const Decision startDecision = Decision.Pending;
        private Decision timeOutDecision;
        private bool decisionMade;

        public Decision Decision
        {
            get; private set;
        }

        public override bool MoveNext()
        {
            bool res = base.MoveNext() && !decisionMade;
            if (!res && !decisionMade)
            {
                MakeDecision(timeOutDecision);
            }
            return res;
        }

        public DecisionTimer() : this(Decision.Pending) { }

        protected DecisionTimer(Decision timeOutDecision) : base()
        {
            this.timeOutDecision = timeOutDecision;
            Decision = startDecision;
        }

        public override void MakeDecision(Decision decision, Card card = null)
        {
            Decision = decision;
            decisionMade = decision != startDecision;
        }

    }

    public class ResponseTimer : DecisionTimer
    {

        public Card ResponseCard { get; private set; }

        public ResponseTimer() : base(Decision.TakeHit) { }

        public override void MakeDecision(Decision decision, Card card)
        {
            base.MakeDecision(decision, card);
            ResponseCard = card;
        }

    }

    public class DyingTimer : DecisionTimer
    {

        private Func<bool> dying;

        public override bool MoveNext()
        {
            return dying();
        }

        public DyingTimer(PlayerController pc) : base()
        {
            dying = () => base.MoveNext() && pc.IsDying;
        }

    }
}