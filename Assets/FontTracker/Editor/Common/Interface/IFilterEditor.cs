namespace Voodoo.Sauce.Font.Common
{
    public interface IFilterEditor<T>
    {
        void OnGUI();
        bool IsValid(T value);
    }
}
