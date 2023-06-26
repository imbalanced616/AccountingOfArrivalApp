using AccountingOfArrivalApp.Classes;
using AccountingOfArrivalApp.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccountingOfArrivalApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageMovements.xaml
    /// </summary>
    public partial class PageMovements : Page
    {
        public PageMovements()
        {
            InitializeComponent();
            dtgMovements.ItemsSource = Movement.ToList();
            CountItems.Text = dtgMovements.Items.Count.ToString();
            LoadFilters();
        }

        public void LoadFilters()
        {
            FilterOnWarehouse.Items.Clear();
            foreach (Warehouses w in Warehouses.ToList())
            {
                MenuItem menuItem = new MenuItem() { Header = w.Name };
                menuItem.Click += (s, e) =>
                {
                    dtgMovements.ItemsSource = Movement.ToListByFilter(menuItem);
                    CountItems.Text = dtgMovements.Items.Count.ToString();
                };
                FilterOnWarehouse.Items.Add(menuItem);
            }
            FilterOnWarehouse.Items.Add(new Separator() { Margin = new Thickness(0, 3, 0, 3) });
            MenuItem menuItemClearWarehouse = new MenuItem() { Header = "Сброс" };
            menuItemClearWarehouse.Click += (s, e) => MenuUpdate_Click(s, e);
            FilterOnWarehouse.Items.Add(menuItemClearWarehouse);
        }

        private void TxbSearch_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txbSearch.Focus();
        }

        private void TxbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbSearch.Text.Count() != 0) dtgMovements.ItemsSource = Movement.ToListBySearch(txbSearch.Text.ToLower());
            else dtgMovements.ItemsSource = Movement.ToList();
            CountItems.Text = dtgMovements.Items.Count.ToString();
        }

        private void MenuUpdate_Click(object sender, RoutedEventArgs e)
        {
            dtgMovements.ItemsSource = Movement.ToList();
            CountItems.Text = dtgMovements.Items.Count.ToString();
            LoadFilters();
            txbSearch.Clear();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageLobby());
        }
    }
}
