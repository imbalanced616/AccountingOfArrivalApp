using AccountingOfArrivalApp.Classes;
using AccountingOfArrivalApp.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccountingOfArrivalApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageStorage.xaml
    /// </summary>
    public partial class PageStorage : Page
    {
        public PageStorage()
        {
            InitializeComponent();
            dtgStorage.ItemsSource = Storage.ToList();
            CountItems.Text = dtgStorage.Items.Count.ToString();
            LoadFilters();
        }

        public void LoadFilters()
        {
            FilterOnType.Items.Clear();
            foreach (Types t in Types.ToList())
            {
                MenuItem menuItem = new MenuItem() { Header = t.Name };
                menuItem.Click += (s, e) =>
                {
                    dtgStorage.ItemsSource = Storage.ToListByFilter(menuItem, 0);
                    CountItems.Text = dtgStorage.Items.Count.ToString();
                };
                FilterOnType.Items.Add(menuItem);
            }

            FilterOnWarehouse.Items.Clear();
            foreach (Warehouses w in Warehouses.ToList())
            {
                MenuItem menuItem = new MenuItem() { Header = w.Name };
                menuItem.Click += (s, e) =>
                {
                    dtgStorage.ItemsSource = Storage.ToListByFilter(menuItem, 1);
                    CountItems.Text = dtgStorage.Items.Count.ToString();
                };
                FilterOnWarehouse.Items.Add(menuItem);
            }
        }

        private void DtgStorage_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dtgStorage.SelectedItem is Storage _selectedItem && _selectedItem.Material.DrawingNumber != "Отсутствует")
                ClassHelper.frmObj.Navigate(new PageMaterialComposition(_selectedItem));
        }

        private void TxbSearch_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txbSearch.Focus();
        }

        private void TxbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbSearch.Text.Count() != 0) dtgStorage.ItemsSource = Storage.ToListBySearch(txbSearch.Text.ToLower());
            else dtgStorage.ItemsSource = Storage.ToList();
            CountItems.Text = dtgStorage.Items.Count.ToString();
        }

        private void MenuUpdate_Click(object sender, RoutedEventArgs e)
        {
            dtgStorage.ItemsSource = Storage.ToList();
            CountItems.Text = dtgStorage.Items.Count.ToString();
            LoadFilters();
            txbSearch.Clear();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageLobby());
        }
    }
}
