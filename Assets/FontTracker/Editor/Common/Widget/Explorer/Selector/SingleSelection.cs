namespace Voodoo.Sauce.Font.Common
{
    public class SingleSelection : AbstractSelector
    {
        protected override int[] GetSelection(int clickedIndex) => Selection = new int[1] { clickedIndex };
    }
}
