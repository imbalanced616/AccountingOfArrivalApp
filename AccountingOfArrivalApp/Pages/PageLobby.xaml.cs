using AccountingOfArrivalApp.Classes;
using System.Windows;
using System.Windows.Controls;

namespace AccountingOfArrivalApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageLobby.xaml
    /// </summary>
    public partial class PageLobby : Page
    {
        public PageLobby()
        {
            InitializeComponent();
            DataContext = User.CurrentUser;
            switch (User.CurrentUser.IdTypeUser)
            {
                case 1:
                    btnDistribution.Visibility = Visibility.Visible;
                    btnMovement.Visibility = Visibility.Visible;
                    btnUserManagement.Visibility = Visibility.Visible;
                    break;
                case 3:
                    btnDistribution.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void BrdPersonalAccount_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) ClassHelper.frmObj.Navigate(new PagePersonalAccount());
        }

        private void MenuPersonalAccount_MouseDown(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PagePersonalAccount());
        }

        private void MenuSignOut_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new LoginPage());
            User.CurrentUser = null;
        }

        private void BtnNomenclature_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageNomenclature());
        }

        private void BtnWarehouses_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageWarehouses());
        }

        private void BtnCounterparties_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageCounterparties());
        }

        private void BtnUserManagement_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageUsers());
        }

        private void BtnStorage_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageStorage());
        }

        private void BtnArrivals_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageArrivals());
        }

        private void BtnDistribution_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageDistributions());
        }

        private void BtnMovement_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageMovements());
        }
    }
}
