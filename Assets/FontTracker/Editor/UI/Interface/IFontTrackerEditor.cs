using Voodoo.Sauce.Font.Common;

namespace Voodoo.Sauce.Font
{
    public interface IFontTrackerEditor : IEditor
    {
        IFontFinderEditor Finder { get; }
        IFontReplacerEditor Replacer { get; }
    }

    public interface IFontFinderEditor : IEditor
    { 
    }

    public interface IFontReplacerEditor : IEditor
    {
    }
}
