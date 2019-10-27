using UnityEngine;

namespace PimPamPum
{
    public abstract class WaitForTimer : WaitFor
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

        protected WaitForTimer()
        {
            mainCorutine = this;
            time = 0;
        }
    }
}