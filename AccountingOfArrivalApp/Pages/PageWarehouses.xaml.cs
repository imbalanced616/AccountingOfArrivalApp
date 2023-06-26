using AccountingOfArrivalApp.Classes;
using AccountingOfArrivalApp.Models;
using AccountingOfArrivalApp.Pages.Add_Edit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccountingOfArrivalApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageWarehouses.xaml
    /// </summary>
    public partial class PageWarehouses : Page
    {
        public PageWarehouses()
        {
            InitializeComponent();
            dtgWarehouses.ItemsSource = Warehouses.ToList();
            CountItems.Text = dtgWarehouses.Items.Count.ToString();
        }

        private void TxbSearch_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txbSearch.Focus();
        }

        private void TxbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbSearch.Text.Count() != 0) dtgWarehouses.ItemsSource = Warehouses.ToListBySearch(txbSearch.Text.ToLower());
            else dtgWarehouses.ItemsSource = Warehouses.ToList();
            CountItems.Text = dtgWarehouses.Items.Count.ToString();
        }

        private void MenuAddOrEdit_Click(object sender, RoutedEventArgs e)
        {
            if (((MenuItem)sender).Header.ToString() == "Добавить") ClassHelper.frmObj.Navigate(new AddEditPageWarehouse(null));
            else if ((Warehouses)dtgWarehouses.SelectedItem != null) ClassHelper.frmObj.Navigate(new AddEditPageWarehouse((Warehouses)dtgWarehouses.SelectedItem));
            else MessageBox.Show("Выберите склад для изменения.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void MenuUpdate_Click(object sender, RoutedEventArgs e)
        {
            dtgWarehouses.ItemsSource = Warehouses.ToList();
            CountItems.Text = dtgWarehouses.Items.Count.ToString();
            txbSearch.Clear();
        }

        private void MenuDelete_Click(object sender, RoutedEventArgs e)
        {
            List<Warehouses> list = dtgWarehouses.SelectedItems.Cast<Warehouses>().ToList();
            if (list.Count == 0)
            {
                MessageBox.Show($"Выберите склады для удаления.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (list.Count == 1)
            {
                if (MessageBox.Show($"Вы точно хотите удалить запись №{list.First().IdWarehouse}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Storage.RemoveAtIdWarehouse(list.First().IdWarehouse);
                        Movement.RemoveAtIdWarehouse(list.First().IdWarehouse);
                        Warehouses.RemoveAtId(list.First().IdWarehouse);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else return;
            }
            if (list.Count > 1)
            {
                if (MessageBox.Show($"Вы точно хотите удалить записи {list.Count}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        foreach (Warehouses w in list)
                        {
                            Storage.RemoveAtIdWarehouse(w.IdWarehouse);
                            Movement.RemoveAtIdWarehouse(w.IdWarehouse);
                            Warehouses.RemoveAtId(w.IdWarehouse);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else return;
            }
            MessageBox.Show("Данные удалены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            dtgWarehouses.ItemsSource = Warehouses.ToList();
            CountItems.Text = dtgWarehouses.Items.Count.ToString();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageLobby());
        }
    }
}
