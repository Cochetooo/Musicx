namespace Musicx.Ui.Core;

public class AbstractFactory<T>(Func<T> factory) : IAbstractFactory<T>
{
    public T Create()
    {
        return factory();
    }
}

public interface IAbstractFactory<out T>
{
    T Create();
}