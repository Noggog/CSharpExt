using System.Reactive.Disposables;

namespace Noggog.Autofac.Validation;

public interface IValidateTracker
{
    IDisposable Track(Type type);
    string State();
}

public class ValidateTracker : IValidateTracker
{
    private Stack<Type> _types = new();
        
    public IDisposable Track(Type type)
    {
        _types.Push(type);
        return Disposable.Create(() => _types.Pop());
    }

    public string State()
    {
        return string.Join($" -> {Environment.NewLine}   ", _types.Reverse());
    }
}