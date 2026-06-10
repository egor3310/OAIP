using System.Windows;
using Contacts.ViewModel;

namespace View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="MainWindow"/>.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainVM();
    }
}