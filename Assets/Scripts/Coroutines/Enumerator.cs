
namespace PimPamPum
{
    public abstract class Enumerator : System.Collections.IEnumerator
    {
        public object Current { get; protected set; }

        public abstract bool MoveNext();

        public virtual void Reset() { }
    }

    public abstract class FirstTimeEnumerator : Enumerator
    {

        private bool first = true;

        protected bool FirstTime
        {
            get
            {
                if (first)
                {
                    first = false;
                    return true;
                }
                return false;
            }
        }
    }
}