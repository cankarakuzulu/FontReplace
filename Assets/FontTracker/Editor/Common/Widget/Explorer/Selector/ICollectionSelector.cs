using System;

namespace Voodoo.Sauce.Font.Common
{
    public interface ICollectionSelector
    {
        event Action<int[]> collectionChanged;

        int[] Selection { get; }
        
        void UpdateSelection(int clickedIndex);
    }
}
