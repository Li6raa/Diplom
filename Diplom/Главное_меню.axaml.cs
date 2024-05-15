using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Diplom;

public partial class Главное_меню : Window
{
    public Главное_меню()
    {
        InitializeComponent();
    }
    
    private void OpenNewWindow(object? sender, RoutedEventArgs routedEventArgs)
    {
        var newWindow = new Форма_Ассортимент();
        newWindow.Show();
        this.Close();
    }
    private void OpenWindow(object? sender, RoutedEventArgs routedEventArgs)
    {
        var newWindow = new Форма_Заявки();
        newWindow.Show();
        this.Close();
    }
    private void exit(object? sender, RoutedEventArgs routedEventArgs)
    {
        this.Close();
    }
}