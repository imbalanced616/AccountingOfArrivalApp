using AccountingOfArrivalApp.Classes;
using AccountingOfArrivalApp.Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AccountingOfArrivalApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для PagePersonalAccount.xaml
    /// </summary>
    public partial class PagePersonalAccount : Page
    {
        readonly Users _currentUser = ClassHelper.db.Users.FirstOrDefault(x => x.idUser == User.CurrentUser.Id);
        public PagePersonalAccount()
        {
            InitializeComponent();
            DataContext = _currentUser;
            if (_currentUser.Photo != null) btnImageClear.Visibility = Visibility.Visible;
            if (_currentUser.idUserTypes == 1) btnUserManagement.Visibility = Visibility.Visible;
        }

        private void BtnDeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Вы точно хотите удалить свой профиль?\n\nПри удалении своего профиля вы будете перенаправлены на страницу авторизации.", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Movement.RemoveAtIdUser(_currentUser.idUser);
                    InvoicesOnArrival.UpdateAtIdUser(_currentUser.idUser);
                    ClassHelper.db.Users.Remove(_currentUser);
                    ClassHelper.db.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else return;
            MessageBox.Show("Аккаунт удалён!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            ClassHelper.frmObj.Navigate(new LoginPage());
            User.CurrentUser = null;
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
                        imageUser.Source = new BitmapImage(new Uri(dlg.FileName.ToString()));
                        FileStream fs = new FileStream(dlg.FileName.ToString(), FileMode.Open, FileAccess.Read);
                        BinaryReader br = new BinaryReader(fs);
                        _currentUser.Photo = br.ReadBytes((int)fs.Length);
                        User.Update(_currentUser);
                        ClassHelper.db.SaveChanges();
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
                    imageUser.Source = new BitmapImage(new Uri(dlg.FileName.ToString()));
                    FileStream fs = new FileStream(dlg.FileName.ToString(), FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    _currentUser.Photo = br.ReadBytes((int)fs.Length);
                    User.Update(_currentUser);
                    ClassHelper.db.SaveChanges();
                    btnImageClear.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void ImageClear_Click(object sender, RoutedEventArgs e)
        {
            _currentUser.Photo = null;
            ClassHelper.db.SaveChanges();
            User.Update(_currentUser);
            imageUser.Source = (ImageSource)FindResource("UnknownUser");
            btnImageClear.Visibility = Visibility.Hidden;
        }

        void Run_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Run run = sender as Run;
                switch (run.Name)
                {
                    case "txtSurname":
                        txbSurname.Visibility = Visibility.Visible;
                        txbSurname.CaretIndex = txbSurname.Text.Length;
                        txbSurname.Focus();
                        break;
                    case "txtName":
                        txbName.Visibility = Visibility.Visible;
                        txbName.CaretIndex = txbName.Text.Length;
                        txbName.Focus();
                        break;
                    case "txtPatronymic":
                        txbPatronymic.Visibility = Visibility.Visible;
                        txbPatronymic.CaretIndex = txbPatronymic.Text.Length;
                        txbPatronymic.Focus();
                        break;
                    case "txtDateOfBirthday":
                        txbDateOfBirthday.Visibility = Visibility.Visible;
                        txbDateOfBirthday.Focus();
                        break;
                    case "txtLogin":
                        txbLogin.Visibility = Visibility.Visible;
                        txbLogin.CaretIndex = txbLogin.Text.Length;
                        txbLogin.Focus();
                        break;
                    case "txtPassword":
                        txbPassword.Visibility = Visibility.Visible;
                        txbPassword.CaretIndex = txbPassword.Text.Length;
                        txbPassword.Focus();
                        break;
                }
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = e.Source as TextBox;
                switch (textBox.Name)
                {
                    case "txbSurname":
                        if (string.IsNullOrWhiteSpace(txbSurname.Text))
                        {
                            MessageBox.Show("Укажите фамилию.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        txbSurname.Visibility = Visibility.Hidden;
                        break;
                    case "txbName":
                        if (string.IsNullOrWhiteSpace(txbName.Text))
                        {
                            MessageBox.Show("Укажите имя.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        txbName.Visibility = Visibility.Hidden;
                        break;
                    case "txbPatronymic":
                        if (string.IsNullOrWhiteSpace(txbPatronymic.Text))
                        {
                            MessageBox.Show("Укажите отчество.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        txbPatronymic.Visibility = Visibility.Hidden;
                        break;
                    case "txbLogin":
                        if (string.IsNullOrWhiteSpace(txbLogin.Text))
                        {
                            MessageBox.Show("Укажите логин.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        txbLogin.Visibility = Visibility.Hidden;
                        break;
                    case "txbPassword":
                        if (string.IsNullOrWhiteSpace(txbPassword.Text))
                        {
                            MessageBox.Show("Укажите пароль.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        txbPassword.Visibility = Visibility.Hidden;
                        break;
                }
                User.Update(_currentUser);
                ClassHelper.db.SaveChanges();
            }
            if (e.Key == Key.Escape)
            {
                TextBox textBox = e.Source as TextBox;
                switch (textBox.Name)
                {
                    case "txbSurname":
                        textBox.Text = _currentUser.Surname;
                        txbSurname.Visibility = Visibility.Hidden;
                        break;
                    case "txbName":
                        textBox.Text = _currentUser.Name;
                        txbName.Visibility = Visibility.Hidden;
                        break;
                    case "txbPatronymic":
                        textBox.Text = _currentUser.Patronymic;
                        txbPatronymic.Visibility = Visibility.Hidden;
                        break;
                    case "txbLogin":
                        textBox.Text = _currentUser.Login;
                        txbLogin.Visibility = Visibility.Hidden;
                        break;
                    case "txbPassword":
                        textBox.Text = _currentUser.Password;
                        txbPassword.Visibility = Visibility.Hidden;
                        break;
                }
            }
        }

        private void DatePicker_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DatePicker datePicker = e.Source as DatePicker;
                if (string.IsNullOrWhiteSpace(datePicker.Text))
                {
                    MessageBox.Show("Укажите дату рождения.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (_currentUser.Age > 100 || _currentUser.Age < 16)
                {
                    MessageBox.Show($"Поле \"Дата рождения\" введено некорректно!\n\nВозраст пользователя не может быть меньше 16 и больше 100.\n\nТекущий возраст: {_currentUser.Age} {_currentUser.AgeType}.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    datePicker.Text = "";
                    return;
                }
                User.CurrentUser.DateOfBirthday = Convert.ToDateTime(datePicker.Text);
                txbDateOfBirthday.Visibility = Visibility.Hidden;
                _currentUser.DateOfBirthday = Convert.ToDateTime(datePicker.Text);
                ClassHelper.db.SaveChanges();
            }
            if (e.Key == Key.Escape)
            {
                DatePicker datePicker = e.Source as DatePicker;
                datePicker.Text = _currentUser.DateOfBirthday.ToString();
                txbDateOfBirthday.Visibility = Visibility.Hidden;
            }
        }

        private void BtnUserManagement_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageUsers());
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageLobby());
        }

        private void BtnSignOut_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new LoginPage());
            User.CurrentUser = null;
        }
    }
}
