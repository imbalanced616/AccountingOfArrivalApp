using AccountingOfArrivalApp.Classes;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AccountingOfArrivalApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        string imgLoc = "пусто";
        public RegistrationPage()
        {
            InitializeComponent();
            txbSurname.Focus();
        }

        private void ImageLoad_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                try
                {
                    OpenFileDialog dlg = new OpenFileDialog { Filter = "Файлы изображений (*.png, *.jpg, *.jpeg, *.bmp)|*.png;*.jpg;*.jpeg;*.bmp|Все файлы (*.*)|*.*", Title = "Выберите фото/изображение пользователя" };
                    if (dlg.ShowDialog() == true)
                    {
                        imgLoc = dlg.FileName.ToString();
                        imageUser.Source = new BitmapImage(new Uri(imgLoc));
                        btnImageClear.Visibility = Visibility.Visible;
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }

        private void ImageLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog { Filter = "Файлы изображений (*.png, *.jpg, *.jpeg, *.bmp)|*.png;*.jpg;*.jpeg;*.bmp|Все файлы (*.*)|*.*", Title = "Выберите фото/изображение пользователя" };
                if (dlg.ShowDialog() == true)
                {
                    imgLoc = dlg.FileName.ToString();
                    imageUser.Source = new BitmapImage(new Uri(imgLoc));
                    btnImageClear.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void ImageClear_Click(object sender, RoutedEventArgs e)
        {
            imageUser.Source = (ImageSource)FindResource("UnknownUser");
            imgLoc = "очистить";
            btnImageClear.Visibility = Visibility.Hidden;
        }

        private void BtnSingIn_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new LoginPage());
        }

        private void TxbSurname_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txbSurname.Focus();
        }

        private void TxbName_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txbName.Focus();
        }

        private void TxbPatronymic_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txbPatronymic.Focus();
        }

        private void TxbLogin_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txbLogin.Focus();
        }

        private void PbPassword_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            pbPassword.Focus();
        }

        private void PbPassword2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            pbPassword2.Focus();
        }

        private void PbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (((PasswordBox)sender).Password != "") txbPass.Visibility = Visibility.Hidden;
            else txbPass.Visibility = Visibility.Visible;
        }

        private void PbPassword2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (((PasswordBox)sender).Password != "") txbPass2.Visibility = Visibility.Hidden;
            else txbPass2.Visibility = Visibility.Visible;
        }

        private void BtnSingUp_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder error = new StringBuilder();
            if (txbSurname.Text == "") error.AppendLine("Поле \"Фамилия\" не заполнено!");
            if (txbName.Text == "") error.AppendLine("Поле \"Имя\" не заполнено!");
            if (txbPatronymic.Text == "") error.AppendLine("Поле \"Отчество\" не заполнено!");
            if (dtpkrDateOfBirthday.Text == "") error.AppendLine("Поле \"Дата рождения\" не заполнено!");
            if (txbLogin.Text == "") error.AppendLine("Поле \"Логин\" не заполнено!");
            if (ClassHelper.db.Users.FirstOrDefault(x => x.Login == txbLogin.Text) != null) error.AppendLine("Пользователь с таким логином уже существует!");
            if (pbPassword.Password == "" || pbPassword2.Password == "") error.AppendLine("Поле \"Пароль\" не заполнено!");
            if (pbPassword.Password != pbPassword2.Password) error.AppendLine("Пароли не совпадают!");
            if (error.Length > 0)
            {
                MessageBox.Show(error.ToString(), "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                if (error.ToString().Contains("логином")) txbLogin.Clear();
                return;
            }
            Users user = new Users() { Login = txbLogin.Text, Password = pbPassword.Password, Surname = txbSurname.Text, Name = txbName.Text, Patronymic = txbPatronymic.Text, DateOfBirthday = dtpkrDateOfBirthday.SelectedDate, idUserTypes = ClassHelper.db.TypesOfUsers.FirstOrDefault(x => x.NameType == "Пользователь").idUserTypes };
            if (user.Age > 100 || user.Age < 16)
            {
                MessageBox.Show($"Поле \"Дата рождения\" введено некорректно!\n\nВозраст пользователя не может быть меньше 16 и больше 100.\n\nТекущий возраст: {user.Age} {user.AgeType}.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                dtpkrDateOfBirthday.Text = "";
                return;
            }
            if (imgLoc != "пусто" && imgLoc != "очистить")
            {
                FileStream fs = new FileStream(imgLoc, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                user.Photo = br.ReadBytes((int)fs.Length);
            }
            if (imgLoc == "очистить") user.Photo = null;
            ClassHelper.db.Users.Add(user);
            try
            {
                ClassHelper.db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("Новый пользователь успешно зарегистрирован!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            User _ = new User(user);
            ClassHelper.frmObj.Navigate(new PageLobby());
        }
    }
}
