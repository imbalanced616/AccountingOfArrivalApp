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

namespace AccountingOfArrivalApp.Pages.Add_Edit
{
    /// <summary>
    /// Логика взаимодействия для AddEditPageUser.xaml
    /// </summary>
    public partial class AddEditPageUser : Page
    {
        private readonly Users _oldItem;
        private readonly Users _currentItem = new Users();
        public AddEditPageUser(Users selectedItem)
        {
            InitializeComponent();
            if (selectedItem != null)
            {
                _currentItem = selectedItem;
                _oldItem = Users.New(selectedItem);
                txtTitle.Text = "Изменение пользователя";
                btnAddEdit.Content = "Изменить";
            }
            cmbTypes.ItemsSource = ClassHelper.db.TypesOfUsers.ToList();
            if (_currentItem.Photo != null) btnImageClear.Visibility = Visibility.Visible;
            DataContext = _currentItem;
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
                        _currentItem.Photo = br.ReadBytes((int)fs.Length);
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
                    _currentItem.Photo = br.ReadBytes((int)fs.Length);
                    btnImageClear.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void ImageClear_Click(object sender, RoutedEventArgs e)
        {
            _currentItem.Photo = null;
            imageUser.Source = (ImageSource)FindResource("UnknownUser");
            btnImageClear.Visibility = Visibility.Hidden;
        }

        private void BtnAddOrEdit_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder error = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentItem.Surname)) error.AppendLine("Укажите фамилию пользователя.");
            if (string.IsNullOrWhiteSpace(_currentItem.Name)) error.AppendLine("Укажите имя пользователя.");
            if (string.IsNullOrWhiteSpace(_currentItem.Patronymic)) error.AppendLine("Укажите отчество пользователя.");
            if (string.IsNullOrWhiteSpace(_currentItem.DateOfBirthday.ToString())) error.AppendLine("Укажите дату рождения пользователя.");
            if (string.IsNullOrWhiteSpace(_currentItem.Login)) error.AppendLine("Укажите логин пользователя.");
            if (ClassHelper.db.Users.FirstOrDefault(x => x.idUser != _currentItem.idUser && x.Login == _currentItem.Login) != null) error.AppendLine("Пользователь с таким логином уже существует!");
            if (string.IsNullOrWhiteSpace(_currentItem.Password)) error.AppendLine("Укажите пароль пользователя.");
            if (string.IsNullOrWhiteSpace(_currentItem.idUserTypes.ToString())) error.AppendLine("Укажите тип пользователя.");
            if (error.Length > 0)
            {
                MessageBox.Show(error.ToString(), "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                if (error.ToString().Contains("логином")) _currentItem.Login = "";
                return;
            }
            if (_currentItem.idUser == 0) ClassHelper.db.Users.Add(_currentItem);
            try
            {
                ClassHelper.db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (_oldItem == null) MessageBox.Show("Новый пользователь успешно добавлен!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            else MessageBox.Show("Пользователь успешно изменён!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            ClassHelper.frmObj.Navigate(new PageUsers());
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Users.UndoСhanges(_currentItem, _oldItem);
            ClassHelper.frmObj.Navigate(new PageUsers());
        }
    }
}
