namespace PimPamPum
{
    public abstract class Enumerator : System.Collections.IEnumerator
    {
        protected bool finished = false;
        public object Current { get; set; }
        public abstract bool MoveNext();
        public virtual void Reset() { }
    }
}