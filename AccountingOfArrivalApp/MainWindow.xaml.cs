using AccountingOfArrivalApp.Classes;
using AccountingOfArrivalApp.Pages;
using System.Windows;

namespace AccountingOfArrivalApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ClassHelper.frmObj = FrameWindow;
            ClassHelper.frmObj.Navigate(new LoginPage());
        }
    }
}
