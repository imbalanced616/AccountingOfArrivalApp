using AccountingOfArrivalApp.Classes;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AccountingOfArrivalApp.Pages.Add_Edit
{
    /// <summary>
    /// Логика взаимодействия для AddEditPageCounterparty.xaml
    /// </summary>
    public partial class AddEditPageCounterparty : Page
    {
        private readonly Counterparties _oldItem;
        private readonly Counterparties _currentItem = new Counterparties();
        public AddEditPageCounterparty(Counterparties selectedItem)
        {
            InitializeComponent();
            if (selectedItem != null)
            {
                _currentItem = selectedItem;
                _oldItem = Counterparties.New(selectedItem);
                txtTitle.Text = "Изменение контрагента";
                btnAddEdit.Content = " Изменить";
            }
            else txbName.Focus();
            DataContext = _currentItem;
        }
        private void BtnAddOrEdit_Click(object sender, RoutedEventArgs e)
        {
            if (txbName.Text != null && txbName.IsFocused) _currentItem.Name = txbName.Text;
            if (txbINN.Text != null && txbINN.IsFocused) _currentItem.INN = txbINN.Text;
            if (txbAddress.Text != null && txbAddress.IsFocused) _currentItem.Address = txbAddress.Text;
            StringBuilder error = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentItem.Name)) error.AppendLine("Укажите название контрагента.");
            if (string.IsNullOrWhiteSpace(_currentItem.INN)) error.AppendLine("Укажите ИНН контрагента.");
            else if (_currentItem.INN.Length > 12 || _currentItem.INN.Length < 12) error.AppendLine("ИНН контрагента должен состоять из 12 цифр.");
            if (string.IsNullOrWhiteSpace(_currentItem.Address)) error.AppendLine("Укажите адрес контрагента.");
            if (error.Length > 0)
            {
                MessageBox.Show(error.ToString(), "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                if (error.ToString().Contains("12")) txbINN.Clear();
                return;
            }
            try
            {
                if (_currentItem.idCounterparty == 0) ClassHelper.db.Counterparties.Add(_currentItem);
                ClassHelper.db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (_oldItem == null) MessageBox.Show("Новый контрагент успешно добавлен!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            else MessageBox.Show("Контрагент успешно изменён!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            ClassHelper.frmObj.Navigate(new PageCounterparties());
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Counterparties.UndoСhanges(_currentItem, _oldItem);
            ClassHelper.frmObj.Navigate(new PageCounterparties());
        }
    }
}
