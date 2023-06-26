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
    /// Логика взаимодействия для AddEditPageNomenclature.xaml
    /// </summary>
    public partial class AddEditPageNomenclature : Page
    {
        private readonly Materials _currentItem = new Materials();
        public AddEditPageNomenclature(Materials selectedItem)
        {
            InitializeComponent();
            if (selectedItem != null)
            {
                _currentItem = selectedItem;
                txtTitle.Text = "Изменение материала";
                btnAddEdit.Content = "Изменить";
            }
            dtgHierarchy.ItemsSource = Hierarchy.ToListByIdMaterial(_currentItem.IdMaterial);
            cmbTypes.ItemsSource = Types.ToList();
            DataContext = _currentItem;
            LoadComboBoxes();
        }

        public void LoadComboBoxes()
        {
            List<Materials> list = Materials.ToList();
            if (!(from m in list select m.Name.ToString()).ToList().Contains(_currentItem.Name) && _currentItem.Name != null) list.Add(_currentItem);
            cmbParent.ItemsSource = list;
            cmbChild.ItemsSource = list;
        }

        private void CmbTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTypes.SelectedIndex == 3) txbDrawingNumber.IsEnabled = false;
            else txbDrawingNumber.IsEnabled = true;
        }

        private void MenuAdd_Click(object sender, RoutedEventArgs e)
        {
            List<Hierarchy> list = dtgHierarchy.ItemsSource as List<Hierarchy>;
            list.Add(new Hierarchy() { IdParent = _currentItem.IdMaterial, IdChild = _currentItem.IdMaterial, Quantity = 1 });
            dtgHierarchy.ItemsSource = null;
            dtgHierarchy.ItemsSource = list;
            LoadComboBoxes();
        }

        private void MenuUpdate_Click(object sender, RoutedEventArgs e)
        {
            List<Hierarchy> list = Hierarchy.ToListByIdMaterial(_currentItem.IdMaterial);
            foreach (Hierarchy h in dtgHierarchy.ItemsSource) if (!list.Contains(h)) list.Add(h);
            dtgHierarchy.ItemsSource = list;
            LoadComboBoxes();
        }

        private void MenuDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dtgHierarchy.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите элемент в таблице, который хотите удалить.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (MessageBox.Show($"Вы точно хотите удалить запись №{dtgHierarchy.SelectedIndex + 1}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Hierarchy h = dtgHierarchy.SelectedItem as Hierarchy;
                try
                {
                    if (Hierarchy.ToList().FirstOrDefault(x => x.IdHierarchy == h.IdHierarchy && x.IdParent == h.IdParent && x.IdChild == h.IdChild && x.Quantity == h.Quantity) != null)
                        Hierarchy.RemoveAtId(h.IdHierarchy);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                MessageBox.Show("Данные удалены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                List<Hierarchy> list = dtgHierarchy.ItemsSource as List<Hierarchy>;
                list.Remove(h);
                dtgHierarchy.ItemsSource = null;
                dtgHierarchy.ItemsSource = list;
            }
        }

        private void BtnAddOrEdit_Click(object sender, RoutedEventArgs e)
        {
            if (txbName.Text != null && txbName.IsFocused) _currentItem.Name = txbName.Text;
            if (txbDrawingNumber.Text != null && txbDrawingNumber.IsFocused) _currentItem.DrawingNumber = txbDrawingNumber.Text;
            StringBuilder error = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentItem.Name)) error.AppendLine("Укажите название материала.");
            if (_currentItem.IdType == 0) error.AppendLine("Укажите тип материала.");
            if (_currentItem.IdType != 4 && string.IsNullOrWhiteSpace(_currentItem.DrawingNumber)) error.AppendLine("Укажите номер чертежа.");
            if (_currentItem.IdType == 4) _currentItem.DrawingNumber = txbDrawingNumber.Text;
            if (_currentItem.Name != null && _currentItem.DrawingNumber != null && _currentItem.DrawingNumber != "Отсутствует" && Materials.FindDuplicate(_currentItem) != null)
            {
                error.AppendLine("Материал с таким номером чертежа уже существует.");
                txbDrawingNumber.Clear();
            }
            foreach (Hierarchy h in dtgHierarchy.ItemsSource)
            {
                if (h.IdParent != _currentItem.IdMaterial && h.IdChild != _currentItem.IdMaterial)
                {
                    error.AppendLine($"Материал \"{_currentItem.Name}\" должен быть родителем или дочерним элементом другого материала.");
                    break;
                }
                if (h.IdParent == _currentItem.IdMaterial && h.IdChild == _currentItem.IdMaterial)
                {
                    error.AppendLine($"Материал \"{_currentItem.Name}\" не может состоять сам из себя.");
                    break;
                }
                if (_currentItem.IdType == 4 && h.IdParent == _currentItem.IdMaterial)
                {
                    error.AppendLine($"Материал \"{_currentItem.Name}\" с типом \"Материал\" не может быть родителем другого материала.");
                    break;
                }
                if (string.IsNullOrWhiteSpace(h.Quantity.ToString()))
                {
                    error.AppendLine($"Количество не может быть пустым.");
                    break;
                }
                if (h.Quantity <= 0)
                {
                    error.AppendLine($"Количество не может быть отрицательным или равным нулю.");
                    break;
                }
            }
            if (error.Length > 0)
            {
                MessageBox.Show(error.ToString(), "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int status;
            try
            {
                status = Materials.AddOrUpdate(_currentItem);
                if (status == 1)
                {
                    int idCurrentMaterial = Materials.Single(_currentItem).IdMaterial;
                    foreach (Hierarchy h in dtgHierarchy.ItemsSource)
                    {
                        if (h.IdParent == 0) h.IdParent = idCurrentMaterial;
                        if (h.IdChild == 0) h.IdChild = idCurrentMaterial;
                        Hierarchy.AddOrUpdate(h);
                    }
                }
                if (status == 2) foreach (Hierarchy _h in dtgHierarchy.ItemsSource) Hierarchy.AddOrUpdate(_h);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (status == 1) MessageBox.Show("Новый материал успешно добавлен!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            if (status == 2) MessageBox.Show("Материал успешно изменён!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            ClassHelper.frmObj.Navigate(new PageNomenclature());
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageNomenclature());
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
