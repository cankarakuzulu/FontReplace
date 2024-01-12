using UnityEngine;

namespace Voodoo.Sauce.Font.Common
{
    public interface IExplorerItem
    {
        Rect Rect { get; }
        bool IsSelected { get; set; }
        void ContextClick();
        void OnGUI();
    }

    public interface IExplorerItem<T> : IExplorerItem, IProvider<T> { }
}
