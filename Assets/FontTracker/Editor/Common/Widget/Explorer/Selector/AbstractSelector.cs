using System;

namespace Voodoo.Sauce.Font.Common
{
    public abstract class AbstractSelector : ICollectionSelector
    {
        public int[] Selection { get; protected set; } = new int[0];

        public event Action<int[]> collectionChanged;

        public void UpdateSelection(int clickedIndex)
        {
            Selection = GetSelection(clickedIndex);
            collectionChanged?.Invoke(Selection);
        }

        protected abstract int[] GetSelection(int clickedIndex);
    }

}
