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
    /// Логика взаимодействия для AddEditMovementDistribution.xaml
    /// </summary>
    public partial class AddEditMovementDistribution : Page
    {
        private readonly InvoicesOnArrival _currentItem = new InvoicesOnArrival();
        private CompositionsOfInvoice _currentSubItem = new CompositionsOfInvoice();
        public AddEditMovementDistribution(InvoicesOnArrival selectedItem)
        {
            InitializeComponent();
            _currentItem = selectedItem;
            txtTitle.DataContext = _currentItem;
            cmbCompositions.ItemsSource = CompositionsOfInvoice.ToContextList(_currentItem.idInvoice);
            btnBack.Click += (s, c) => { ClassHelper.frmObj.Navigate(new PageArrivals()); };
        }

        public AddEditMovementDistribution(CompositionsOfInvoice selectedSubItem)
        {
            InitializeComponent();
            _currentItem = ClassHelper.db.InvoicesOnArrival.FirstOrDefault(x => x.idInvoice == selectedSubItem.idInvoice);
            txtTitle.DataContext = _currentItem;
            cmbCompositions.ItemsSource = CompositionsOfInvoice.ToContextList(_currentItem.idInvoice);
            cmbCompositions.SelectedItem = selectedSubItem;
            btnBack.Click += (s, c) => { ClassHelper.frmObj.Navigate(new PageDistributions()); };
        }

        private void CmbCompositions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_currentSubItem != null) _currentSubItem.QuantityDistributed = 0;
            _currentSubItem = (CompositionsOfInvoice)cmbCompositions.SelectedItem;
            txtQuantity.DataContext = _currentSubItem;
            dtgStorageToMovement.ItemsSource = Storage.ToContextListAndAllWarehouse(_currentSubItem);
        }

        private void BtnDistribute_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder error = new StringBuilder();
            List<Storage> list = dtgStorageToMovement.ItemsSource as List<Storage>;
            CompositionsOfInvoice c = (CompositionsOfInvoice)cmbCompositions.SelectedItem;
            if (c.Background == "Red") error.AppendLine("Распределённое количество отличается от количества для распределения данной позиции.");
            for (int i = 0; i < list.Count; i++) if (list[i].QuantityDistribute < 0) error.AppendLine($"Строка №{i + 1}: Поле \"Количество на распределение\" не может быть отрицательным.");
            if (error.Length > 0)
            {
                MessageBox.Show(error.ToString(), "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                foreach (Storage s in list) Movement.AddOrUpdateOrDelete(new Movement() { IdWarehouse = s.IdWarehouse, IdComposition = c.idComposition, PartOfQuantity = s.QuantityDistribute, ArrivalOrExpenditure = false });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            for (int i = 0; i < cmbCompositions.Items.Count; i++)
            {
                if (((CompositionsOfInvoice)cmbCompositions.Items[i]).Background == "Red")
                {
                    cmbCompositions.SelectedIndex = i;
                    return;
                }
            }
            MessageBox.Show("Все позиции из этой накладной распределены.\n\nРаспределение данной накладной можно провести и закрыть.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDistributedInvoice_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder error = new StringBuilder();

            List<Storage> list = dtgStorageToMovement.ItemsSource as List<Storage>;
            CompositionsOfInvoice c = (CompositionsOfInvoice)cmbCompositions.SelectedItem;
            if (c.Background == "Red") error.AppendLine("Распределённое количество отличается от количества для распределения данной позиции.");
            for (int i = 0; i < list.Count; i++) if (list[i].QuantityDistribute < 0) error.AppendLine($"Строка №{i + 1}: Поле \"Количество на распределение\" не может быть отрицательным.");
            if (error.Length > 0)
            {
                MessageBox.Show(error.ToString(), "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                foreach (Storage s in list) Movement.AddOrUpdateOrDelete(new Movement() { IdWarehouse = s.IdWarehouse, IdComposition = c.idComposition, PartOfQuantity = s.QuantityDistribute, ArrivalOrExpenditure = false });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            for (int i = 0; i < cmbCompositions.Items.Count; i++)
            {
                CompositionsOfInvoice com = cmbCompositions.Items[i] as CompositionsOfInvoice;
                if (com.Background == "Red") error.AppendLine($"Материал \"{com.Materials.Name}\" не распределён.");
            }
            if (error.Length > 0)
            {
                MessageBox.Show(error.ToString(), "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show($"Вы точно хотите провести и закрыть приходную накладную №{_currentItem.idInvoice}?\n\nПосле проводки и закрытия поступление больше не может быть отредактировано.", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            try
            {
                foreach (Movement m in Movement.ToContextList(_currentItem.idInvoice)) Storage.AddOrUpdate(m);
                _currentItem.DistributedInvoice = true;
                ClassHelper.db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("Поступление успешно проведено и закрыто!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            ClassHelper.frmObj.Navigate(new PageArrivals());
        }

        private void DtgStorageToMovement_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            ((CompositionsOfInvoice)cmbCompositions.SelectedItem).QuantityDistributed = ((List<Storage>)dtgStorageToMovement.ItemsSource).Sum(x => x.QuantityDistribute);
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
