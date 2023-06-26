using AccountingOfArrivalApp.Classes;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccountingOfArrivalApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            txbLogin.Focus();
        }

        private void BtnSignIn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder error = new StringBuilder();
            if (txbLogin.Text == "") error.AppendLine("Поле \"Логин\" не заполнено!");
            if (pbPassword.Password == "") error.AppendLine("Поле \"Пароль\" не заполнено!");
            if (error.Length > 0)
            {
                MessageBox.Show(error.ToString(), "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Users user = new Users();
            try
            {
                user = ClassHelper.db.Users.FirstOrDefault(x => x.Login == txbLogin.Text && x.Password == pbPassword.Password);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            User _ = new User(user);
            ClassHelper.frmObj.Navigate(new PageLobby());
        }

        private void BtnSignUp_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new RegistrationPage());
        }

        private void TxbLogin_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txbLogin.Focus();
        }

        private void PbPassword_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            pbPassword.Focus();
        }

        private void PbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (((PasswordBox)sender).Password != "") txbPass.Visibility = Visibility.Hidden;
            else txbPass.Visibility = Visibility.Visible;
        }
    }
}
