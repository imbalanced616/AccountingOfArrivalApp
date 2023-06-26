using AccountingOfArrivalApp.Classes;
using AccountingOfArrivalApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccountingOfArrivalApp.Pages.Add_Edit
{
    /// <summary>
    /// Логика взаимодействия для AddEditPageArrival.xaml
    /// </summary>
    public partial class AddEditPageArrival : Page
    {
        private readonly InvoicesOnArrival _currentItem = new InvoicesOnArrival();
        private readonly int status = 1;
        public AddEditPageArrival(InvoicesOnArrival selectedItem)
        {
            InitializeComponent();
            if (selectedItem != null)
            {
                _currentItem = selectedItem;
                txtTitle.Text = "Изменение поставки";
                btnAddEdit.Content = "Изменить";
                status = 2;
            }
            else
            {
                _currentItem.DeliveryDate = DateTime.Now;
                _currentItem.DistributedInvoice = false;
            }
            cmbCounterparties.ItemsSource = ClassHelper.db.Counterparties.ToList();
            cmbUsers.ItemsSource = ClassHelper.db.Users.ToList();
            if (User.CurrentUser.IdTypeUser == 1) cmbUsers.IsEnabled = true;
            _currentItem.idUser = User.CurrentUser.Id;
            dtgComposition.ItemsSource = CompositionsOfInvoice.ToContextList(_currentItem.idInvoice);
            cmbMaterials.ItemsSource = Materials.ToList();
            DataContext = _currentItem;
            CountItems.Text = dtgComposition.Items.Count.ToString();
        }

        private void MenuAdd_Click(object sender, RoutedEventArgs e)
        {
            List<CompositionsOfInvoice> list = dtgComposition.ItemsSource as List<CompositionsOfInvoice>;
            list.Add(new CompositionsOfInvoice() { idInvoice = _currentItem.idInvoice, Quantity = 1, PricePerUnit = 1 });
            _currentItem.Amount = list.Sum(x => x.Amount);
            dtgComposition.ItemsSource = null;
            dtgComposition.ItemsSource = list;
            CountItems.Text = dtgComposition.Items.Count.ToString();
        }

        private void MenuUpdate_Click(object sender, RoutedEventArgs e)
        {
            List<CompositionsOfInvoice> list = CompositionsOfInvoice.ToContextList(_currentItem.idInvoice);
            foreach (CompositionsOfInvoice c in dtgComposition.ItemsSource)
                if (!list.Contains(c)) list.Add(c);
            _currentItem.Amount = list.Sum(x => x.Amount);
            dtgComposition.ItemsSource = list;
            CountItems.Text = dtgComposition.Items.Count.ToString();
        }

        private void MenuDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dtgComposition.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите запись в таблице, который хотите удалить.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (MessageBox.Show($"Вы точно хотите удалить запись №{dtgComposition.SelectedIndex + 1}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                CompositionsOfInvoice c = dtgComposition.SelectedItem as CompositionsOfInvoice;
                try
                {
                    if (ClassHelper.db.CompositionsOfInvoice.FirstOrDefault(x => x.idComposition == c.idComposition && x.idInvoice == c.idInvoice &&
                    x.idMaterial == c.idMaterial && x.Quantity == c.Quantity && x.PricePerUnit == c.PricePerUnit) != null)
                        CompositionsOfInvoice.RemoveAtId(c.idComposition);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                MessageBox.Show("Данные удалены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                List<CompositionsOfInvoice> list = dtgComposition.ItemsSource as List<CompositionsOfInvoice>;
                list.Remove(c);
                _currentItem.Amount = list.Sum(x => x.Amount);
                dtgComposition.ItemsSource = null;
                dtgComposition.ItemsSource = list;
                CountItems.Text = dtgComposition.Items.Count.ToString();
            }
        }

        private void BtnAddOrEdit_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder error = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentItem.idCounterparty.ToString())) error.AppendLine("Укажите поставщика.");
            List<CompositionsOfInvoice> list = dtgComposition.ItemsSource as List<CompositionsOfInvoice>;
            if (list.Count == 0) error.AppendLine("Укажите в составе хотя бы одну запись.");
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].idMaterial == null) error.AppendLine($"Строка №{i + 1}: Поле \"Материал\" не заполнено.");
                if (list[i].Quantity == null) error.AppendLine($"Строка №{i + 1}: Поле \"Количество\" не заполнено.");
                if (list[i].Quantity <= 0) error.AppendLine($"Строка №{i + 1}: Поле \"Количество\" не может быть отрицательным или равным нулю.");
                if (list[i].PricePerUnit == null) error.AppendLine($"Строка №{i + 1}: Поле \"Цена за единицу\" не заполнено.");
                if (list[i].PricePerUnit <= 0) error.AppendLine($"Строка №{i + 1}: Поле \"Цена за единицу\" не может быть отрицательным или равным нулю.");

            }
            if (error.Length > 0)
            {
                MessageBox.Show(error.ToString(), "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                if (_currentItem.idInvoice == 0)
                {
                    ClassHelper.db.InvoicesOnArrival.Add(_currentItem);
                    ClassHelper.db.SaveChanges();
                    foreach (CompositionsOfInvoice c in list) c.idInvoice = _currentItem.idInvoice;
                }
                ClassHelper.db.SaveChanges();
                foreach (CompositionsOfInvoice c in list) CompositionsOfInvoice.AddOrUpdate(c);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (status == 1) MessageBox.Show("Новая поставка успешно добавлена!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            else MessageBox.Show("Поставка успешно изменена!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            ClassHelper.frmObj.Navigate(new PageArrivals());
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            _currentItem.Amount = 0;
            ClassHelper.frmObj.Navigate(new PageArrivals());
        }

        private void DtgComposition_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            _currentItem.Amount = (dtgComposition.ItemsSource as List<CompositionsOfInvoice>).Sum(x => x.Amount);
        }

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) ClassHelper.GridColumnFastEdit(sender as DataGridCell, e);
        }

        private void DataGridCell_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ClassHelper.GridColumnFastEdit(sender as DataGridCell, e);
        }
    }
}
