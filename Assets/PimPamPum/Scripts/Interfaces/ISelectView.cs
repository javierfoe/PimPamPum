using UnityEngine.EventSystems;

namespace PimPamPum
{
    public interface IClickView : IPointerClickHandler
    {
        void EnableClick(bool value);
        void Click();
    }
}
