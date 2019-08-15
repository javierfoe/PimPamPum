
namespace PimPamPum
{
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