using System;
using System.Collections;
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

        protected virtual void MakeDecision(int player, Card card, Decision decision) { }

        public abstract bool MoveNext();

        public void Reset() { }

    }

    public class DecisionTimer : DecisionMaking
    {

        private float time, maxTime;
        private Decision decision;

        public bool TimeUp
        {
            get; private set;
        }

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
            time += Time.deltaTime;
            bool timer = time < maxTime;
            TimeUp = !timer;
            return timer && !DecisionMade;
        }

        protected DecisionTimer(float maxTime, Decision decision)
        {
            time = 0;
            this.maxTime = maxTime;
            this.decision = decision;
            Decision = decision;
        }

        protected override void MakeDecision(int player, Card card, Decision decision)
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