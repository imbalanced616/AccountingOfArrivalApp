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
    /// Логика взаимодействия для PageNomenclature.xaml
    /// </summary>
    public partial class PageNomenclature : Page
    {
        public PageNomenclature()
        {
            InitializeComponent();
            LoadDataGrid();
            LoadFilters();
        }

        public void LoadDataGrid()
        {
            List<Materials> list = Materials.ToList();
            foreach (Materials m in list)
                if (m.DrawingNumber == null)
                    m.DrawingNumber = "Отсутствует";
            dtgNomenclature.ItemsSource = list;
            CountItems.Text = dtgNomenclature.Items.Count.ToString();
        }

        public void LoadFilters()
        {
            Filters.Items.Clear();
            foreach (Types t in Types.ToList())
            {
                MenuItem menuItem = new MenuItem(){ Header = t.Name };
                menuItem.Click += (s, e) =>
                {
                    dtgNomenclature.ItemsSource = Materials.ToListOnTypeInMenuItem(menuItem);
                    CountItems.Text = dtgNomenclature.Items.Count.ToString();
                };
                Filters.Items.Add(menuItem);
            }
            Filters.Items.Add(new Separator() { Margin = new Thickness(0, 3, 0, 3) });
            MenuItem menuItemClear = new MenuItem() { Header = "Сброс" };
            menuItemClear.Click += (s, e) => MenuUpdate_Click(s, e);
            Filters.Items.Add(menuItemClear);
        }

        private void DtgNomenclature_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dtgNomenclature.SelectedItem is Materials _selectedItem && _selectedItem.DrawingNumber != "Отсутствует")
                ClassHelper.frmObj.Navigate(new PageMaterialComposition(_selectedItem));
        }

        private void TxbSearch_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txbSearch.Focus();
        }

        private void TxbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbSearch.Text.Count() != 0)
            {
                dtgNomenclature.ItemsSource = Materials.ToListBySearch(txbSearch.Text.ToLower());
                CountItems.Text = dtgNomenclature.Items.Count.ToString();
            }
            else LoadDataGrid();
        }

        private void MenuAddOrEdit_Click(object sender, RoutedEventArgs e)
        {
            if (((MenuItem)sender).Header.ToString() == "Добавить") ClassHelper.frmObj.Navigate(new AddEditPageNomenclature(null));
            else if ((Materials)dtgNomenclature.SelectedItem != null) ClassHelper.frmObj.Navigate(new AddEditPageNomenclature((Materials)dtgNomenclature.SelectedItem));
            else MessageBox.Show("Выберите материал для изменения.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void MenuUpdate_Click(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
            LoadFilters();
            txbSearch.Clear();
        }

        private void MenuDelete_Click(object sender, RoutedEventArgs e)
        {
            List<Materials> list = dtgNomenclature.SelectedItems.Cast<Materials>().ToList();
            if (list.Count == 0)
            {
                MessageBox.Show($"Выберите материалы для удаления.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (list.Count == 1)
            {
                if (MessageBox.Show($"Вы точно хотите удалить запись №{list.First().IdMaterial}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Storage.RemoveAtIdMaterial(list.First().IdMaterial);
                        Hierarchy.RemoveAtIdMaterial(list.First().IdMaterial);
                        Materials.RemoveAtId(list.First().IdMaterial);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        foreach (Materials m in list)
                        {
                            Storage.RemoveAtIdMaterial(m.IdMaterial);
                            Hierarchy.RemoveAtIdMaterial(m.IdMaterial);
                            Materials.RemoveAtId(m.IdMaterial);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else return;
            }
            MessageBox.Show("Данные удалены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadDataGrid();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageLobby());
        }
    }
}
