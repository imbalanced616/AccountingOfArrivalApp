using AccountingOfArrivalApp.Classes;
using AccountingOfArrivalApp.Models;
using AccountingOfArrivalApp.Pages.Add_Edit;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccountingOfArrivalApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageUsers.xaml
    /// </summary>
    public partial class PageUsers : Page
    {
        public PageUsers()
        {
            InitializeComponent();
            lstvUsers.ItemsSource = Users.ToList();
            CountItems.Text = lstvUsers.Items.Count.ToString();
            LoadFilters();
        }

        public void LoadFilters()
        {
            FilterOnType.Items.Clear();
            foreach (TypesOfUsers t in ClassHelper.db.TypesOfUsers.ToList())
            {
                MenuItem menuItem = new MenuItem() { Header = t.NameType };
                menuItem.Click += (s, e) =>
                {
                    lstvUsers.ItemsSource = Users.ToListByFilter(menuItem, 0);
                    CountItems.Text = lstvUsers.Items.Count.ToString();
                };
                FilterOnType.Items.Add(menuItem);
            }
        }

        private void TxbSearch_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txbSearch.Focus();
        }

        private void TxbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbSearch.Text.Count() != 0) lstvUsers.ItemsSource = Users.ToListBySearch(txbSearch.Text.ToLower());
            else lstvUsers.ItemsSource = Users.ToList();
            CountItems.Text = lstvUsers.Items.Count.ToString();
        }

        private void MenuAddOrEdit_Click(object sender, RoutedEventArgs e)
        {
            if (((MenuItem)sender).Header.ToString() == "Добавить") ClassHelper.frmObj.Navigate(new AddEditPageUser(null));
            else if (lstvUsers.SelectedItem is Users u)
            {
                if (u.idUser != User.CurrentUser.Id) ClassHelper.frmObj.Navigate(new AddEditPageUser(u));
                else if (MessageBox.Show($"Изменение своего профиля можно осуществить в личном кабинете.\n\nПерейти в него?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)ClassHelper.frmObj.Navigate(new PagePersonalAccount());
            }
            else MessageBox.Show("Выберите пользователя для изменения.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void MenuUpdate_Click(object sender, RoutedEventArgs e)
        {
            lstvUsers.ItemsSource = Users.ToList();
            CountItems.Text = lstvUsers.Items.Count.ToString();
            LoadFilters();
            txbSearch.Clear();
        }

        private void MenuSort_Click(object sender, RoutedEventArgs e)
        {
            lstvUsers.ItemsSource = Users.ToListBySort((MenuItem)((MenuItem)sender).Parent, (MenuItem)sender);
        }

        private void MenuFilter_Click(object sender, RoutedEventArgs e)
        {
            lstvUsers.ItemsSource = Users.ToListByFilter((MenuItem)sender, 1);
        }

        private void MenuDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!(lstvUsers.SelectedItem is Users u))
            {
                MessageBox.Show($"Выберите пользователя для удаления.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string s;
            if (u.idUser == User.CurrentUser.Id) s = $"Вы точно хотите удалить свой профиль?\n\nПри удалении своего профиля вы будете перенаправлены на страницу авторизации.";
            else s = $"Вы точно хотите удалить пользователя №{u.idUser}?";
            if (MessageBox.Show(s, "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Movement.RemoveAtIdUser(u.idUser);
                    InvoicesOnArrival.UpdateAtIdUser(u.idUser);
                    ClassHelper.db.Users.Remove(u);
                    ClassHelper.db.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else return;
            MessageBox.Show("Данные удалены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            if (u.idUser != User.CurrentUser.Id)
            {
                lstvUsers.ItemsSource = Users.ToList();
                CountItems.Text = lstvUsers.Items.Count.ToString();
            }
            else
            {
                ClassHelper.frmObj.Navigate(new LoginPage());
                User.CurrentUser = null;
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageLobby());
        }
    }
}
