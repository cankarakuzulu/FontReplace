using System;
using System.Collections.Generic;

namespace Voodoo.Sauce.Font.Common
{
    public interface IExplorerEditor<T> : IEditor
    {
        IFilterEditor<T> Filter { get; }
        List<T> Selection { get; }

        event Action<T[]> selectionChanged;

        void Fill(List<IExplorerItem<T>> editors);
    }
}
