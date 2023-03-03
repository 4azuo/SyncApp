
using System.Windows;

public abstract class CWindow<T> : Window where T : CWindowData, new()
{
    public T WindowData { get; private set; } = new T();

    public CWindow()
    {
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        Loaded += (s, e) =>
        {
            DataContext = WindowData;
            WindowData.InitWindow(this);
        };
    }
}