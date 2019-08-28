using UnityEngine;

namespace PimPamPum
{
    public interface IDropView : IClickView
    {
        GameObject gameObject { get; }
        IPlayerView IPlayerView { get; set; }
        int PlayerNumber { get; }
        bool Droppable { get; set; }
        Drop DropEnum { get; }
        int DropIndex { get; }
        void SetTargetable(bool value);
        void Highlight(bool value);
    }
}