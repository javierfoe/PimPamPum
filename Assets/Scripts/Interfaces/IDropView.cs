using UnityEngine;

namespace PimPamPum
{
    public interface IDropView
    {
        GameObject GameObject();
        void SetTargetable(bool value);
        bool GetDroppable();
        void SetDroppable(bool value);
        Drop GetDropEnum();
        int GetDropIndex();
    }
}