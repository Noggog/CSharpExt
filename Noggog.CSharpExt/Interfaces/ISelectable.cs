namespace Noggog;

public interface ISelectableItem<TItem> : ISelectable, ISelectedItem<TItem>
{
    new TItem Item { get; set; }
    new bool IsSelected { get; set; }
}
    
public interface ISelectable : ISelected
{
    new bool IsSelected { get; set; }
}
    
public interface ISelectedItem<out TItem> : ISelected
{
    TItem Item { get; }
}
    
public interface ISelected
{
    bool IsSelected { get; }
}