namespace PimPamPum
{
    public abstract class Enumerator : System.Collections.IEnumerator
    {
        public object Current { get; protected set; }

        public abstract bool MoveNext();

        public virtual void Reset() { }
    }
}