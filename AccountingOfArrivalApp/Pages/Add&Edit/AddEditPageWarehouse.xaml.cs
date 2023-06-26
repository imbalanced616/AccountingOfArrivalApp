using AccountingOfArrivalApp.Classes;
using AccountingOfArrivalApp.Models;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AccountingOfArrivalApp.Pages.Add_Edit
{
    /// <summary>
    /// Логика взаимодействия для AddEditPageWarehouse.xaml
    /// </summary>
    public partial class AddEditPageWarehouse : Page
    {
        private readonly Warehouses _currentItem = new Warehouses();
        public AddEditPageWarehouse(Warehouses selectedItem)
        {
            InitializeComponent();
            if (selectedItem != null)
            {
                _currentItem = selectedItem;
                txtTitle.Text = "Изменение склада";
                btnAddEdit.Content = "Изменить";
            }
            else txbName.Focus();
            DataContext = _currentItem;
        }

        private void BtnAddOrEdit_Click(object sender, RoutedEventArgs e)
        {
            if (txbName.Text != null && txbName.IsFocused) _currentItem.Name = txbName.Text;
            if (txbAddress.Text != null && txbAddress.IsFocused) _currentItem.Address = txbAddress.Text;
            StringBuilder error = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentItem.Name)) error.AppendLine("Укажите название склада.");
            if (string.IsNullOrWhiteSpace(_currentItem.Address)) error.AppendLine("Укажите адрес склада.");
            if (error.Length > 0)
            {
                MessageBox.Show(error.ToString(), "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int status;
            try
            {
                status = Warehouses.AddOrUpdate(_currentItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (status == 1) MessageBox.Show("Новый склад успешно добавлен!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            if (status == 2) MessageBox.Show("Склад успешно изменён!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            ClassHelper.frmObj.Navigate(new PageWarehouses());
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageWarehouses());
        }
    }
}
