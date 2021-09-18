namespace Noggog
{
    public interface ISelectable : ISelected
    {
        new bool IsSelected { get; set; }
    }
    
    public interface ISelected
    {
        bool IsSelected { get; }
    }
}