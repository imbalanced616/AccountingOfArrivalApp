using AccountingOfArrivalApp.Classes;
using AccountingOfArrivalApp.Models;
using AccountingOfArrivalApp.Pages.Add_Edit;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccountingOfArrivalApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageDistributions.xaml
    /// </summary>
    public partial class PageDistributions : Page
    {
        public PageDistributions()
        {
            InitializeComponent();
            dtgDistribution.ItemsSource = CompositionsOfInvoice.ToList();
            CountItems.Text = dtgDistribution.Items.Count.ToString();
            LoadFilters();
        }

        public void LoadFilters()
        {
            FilterOnIdInvoice.Items.Clear();
            if (ClassHelper.db.InvoicesOnArrival.ToList().Count > 0)
            {
                foreach (InvoicesOnArrival i in ClassHelper.db.InvoicesOnArrival.ToList())
                {
                    MenuItem menuItem = new MenuItem() { Header = $"Поставка №{i.idInvoice}" };
                    menuItem.Click += (s, e) =>
                    {
                        dtgDistribution.ItemsSource = CompositionsOfInvoice.ToListByFilter(menuItem, 0);
                        CountItems.Text = dtgDistribution.Items.Count.ToString();
                    };
                    FilterOnIdInvoice.Items.Add(menuItem);
                }
            }
            else
            {
                MenuItem menuItem = new MenuItem() { Header = $"Поставки отсутствуют..." };
                menuItem.Click += (s, e) => { MessageBox.Show("Активных поставок нет.\n\nСоздайте поставку для фильтрации движений по номеру поставки.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning); };
                FilterOnIdInvoice.Items.Add(menuItem);
            }

            FilterOnType.Items.Clear();
            foreach (Types t in Types.ToList())
            {
                MenuItem menuItem = new MenuItem() { Header = t.Name };
                menuItem.Click += (s, e) =>
                {
                    dtgDistribution.ItemsSource = CompositionsOfInvoice.ToListByFilter(menuItem, 1);
                    CountItems.Text = dtgDistribution.Items.Count.ToString();
                };
                FilterOnType.Items.Add(menuItem);
            }

            FilterOnDistribution.Items.Clear();
            MenuItem menuItemGreen = new MenuItem() { Header = "Распределено" };
            menuItemGreen.Click += (s, e) =>
            {
                dtgDistribution.ItemsSource = CompositionsOfInvoice.ToListByFilter(menuItemGreen, 2);
                CountItems.Text = dtgDistribution.Items.Count.ToString();
            };
            FilterOnDistribution.Items.Add(menuItemGreen);
            MenuItem menuItemRed = new MenuItem() { Header = "Не распределено" };
            menuItemRed.Click += (s, e) =>
            {
                dtgDistribution.ItemsSource = CompositionsOfInvoice.ToListByFilter(menuItemRed, 2);
                CountItems.Text = dtgDistribution.Items.Count.ToString();
            };
            FilterOnDistribution.Items.Add(menuItemRed);
        }

        private void MenuDistribute_Click(object sender, RoutedEventArgs e)
        {
            if (dtgDistribution.SelectedItem is CompositionsOfInvoice composition)
                if (ClassHelper.db.InvoicesOnArrival.FirstOrDefault(x => x.idInvoice == composition.idInvoice).DistributedInvoice == true) MessageBox.Show($"Позиция \"{composition.Materials?.Name}\" из поставки №{composition.idInvoice} не может быть распределена, т.к. поставка закрыта.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                else ClassHelper.frmObj.Navigate(new AddEditMovementDistribution(composition));
            else MessageBox.Show("Выберите позицию для распределения.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void TxbSearch_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txbSearch.Focus();
        }

        private void TxbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbSearch.Text.Count() != 0) dtgDistribution.ItemsSource = CompositionsOfInvoice.ToListBySearch(txbSearch.Text.ToLower());
            else dtgDistribution.ItemsSource = CompositionsOfInvoice.ToList();
            CountItems.Text = dtgDistribution.Items.Count.ToString();
        }

        private void MenuUpdate_Click(object sender, RoutedEventArgs e)
        {
            dtgDistribution.ItemsSource = CompositionsOfInvoice.ToList();
            CountItems.Text = dtgDistribution.Items.Count.ToString();
            LoadFilters();
            txbSearch.Clear();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageLobby());
        }
    }
}
