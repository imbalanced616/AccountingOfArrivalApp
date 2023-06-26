using AccountingOfArrivalApp.Classes;
using AccountingOfArrivalApp.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AccountingOfArrivalApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageArrivalComposition.xaml
    /// </summary>
    public partial class PageArrivalComposition : Page
    {
        private readonly InvoicesOnArrival _currentItem = new InvoicesOnArrival();
        public PageArrivalComposition(InvoicesOnArrival selectedItem)
        {
            InitializeComponent();
            _currentItem = selectedItem;
            DataContext = _currentItem;
            dtgArrivalComposition.ItemsSource = CompositionsOfInvoice.ToContextList(_currentItem.idInvoice);
            CountItems.Text = dtgArrivalComposition.Items.Count.ToString();
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
                    dtgArrivalComposition.ItemsSource = CompositionsOfInvoice.ToListByFilter(menuItem, 3, dtgArrivalComposition.ItemsSource as List<CompositionsOfInvoice>);
                    CountItems.Text = dtgArrivalComposition.Items.Count.ToString();
                };
                FilterOnType.Items.Add(menuItem);
            }
            FilterOnType.Items.Add(new Separator() { Margin = new Thickness(0, 3, 0, 3) });
            MenuItem menuItemClearDistribution = new MenuItem() { Header = "Сброс" };
            menuItemClearDistribution.Click += (s, e) => MenuUpdate_Click(s, e);
            FilterOnType.Items.Add(menuItemClearDistribution);
        }

        private void MenuUpdate_Click(object sender, RoutedEventArgs e)
        {
            dtgArrivalComposition.ItemsSource = CompositionsOfInvoice.ToContextList(_currentItem.idInvoice);
            CountItems.Text = dtgArrivalComposition.Items.Count.ToString();
            LoadFilters();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageArrivals());
        }
    }
}
