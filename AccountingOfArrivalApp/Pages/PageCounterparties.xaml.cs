using AccountingOfArrivalApp.Classes;
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
    /// Логика взаимодействия для PageCounterparties.xaml
    /// </summary>
    public partial class PageCounterparties : Page
    {
        public PageCounterparties()
        {
            InitializeComponent();
            dtgCounterparties.ItemsSource = ClassHelper.db.Counterparties.ToList();
            CountItems.Text = dtgCounterparties.Items.Count.ToString();
        }

        private void TxbSearch_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txbSearch.Focus();
        }

        private void TxbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbSearch.Text.Count() != 0) dtgCounterparties.ItemsSource = ClassHelper.db.Counterparties.Where(x => x.Name.ToLower().Contains(txbSearch.Text.ToLower())
            || x.INN.ToLower().Contains(txbSearch.Text.ToLower()) || x.Address.ToLower().Contains(txbSearch.Text.ToLower())).ToList();
            else dtgCounterparties.ItemsSource = ClassHelper.db.Counterparties.ToList();
            CountItems.Text = dtgCounterparties.Items.Count.ToString();
        }

        private void MenuAddOrEdit_Click(object sender, RoutedEventArgs e)
        {
            if (((MenuItem)sender).Header.ToString() == "Добавить") ClassHelper.frmObj.Navigate(new AddEditPageCounterparty(null));
            else if (dtgCounterparties.SelectedItem is Counterparties c) ClassHelper.frmObj.Navigate(new AddEditPageCounterparty(c));
            else MessageBox.Show("Выберите контрагента для изменения.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void MenuFilter_Click(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Header.ToString())
            {
                case "ООО":
                    dtgCounterparties.ItemsSource = ClassHelper.db.Counterparties.ToList().Where(x => x.Name.Split(' ')[0] == "ООО");
                    break;
                case "АО":
                    dtgCounterparties.ItemsSource = ClassHelper.db.Counterparties.ToList().Where(x => x.Name.Split(' ')[0] == "АО");
                    break;
                case "ПАО":
                    dtgCounterparties.ItemsSource = ClassHelper.db.Counterparties.ToList().Where(x => x.Name.Split(' ')[0] == "ПАО");
                    break;
                case "ЗАО":
                    dtgCounterparties.ItemsSource = ClassHelper.db.Counterparties.ToList().Where(x => x.Name.Split(' ')[0] == "ЗАО");
                    break;
                case "НКО":
                    dtgCounterparties.ItemsSource = ClassHelper.db.Counterparties.ToList().Where(x => x.Name.Split(' ')[0] == "НКО");
                    break;
                case "ИП":
                    dtgCounterparties.ItemsSource = ClassHelper.db.Counterparties.ToList().Where(x => x.Name.Split(' ')[0] == "ИП");
                    break;
                case "ОП":
                    dtgCounterparties.ItemsSource = ClassHelper.db.Counterparties.ToList().Where(x => x.Name.Split(' ')[0] == "ОП");
                    break;
                default:
                    dtgCounterparties.ItemsSource = ClassHelper.db.Counterparties.ToList();
                    break;
            }
        }

        private void MenuUpdate_Click(object sender, RoutedEventArgs e)
        {
            dtgCounterparties.ItemsSource = ClassHelper.db.Counterparties.ToList();
            CountItems.Text = dtgCounterparties.Items.Count.ToString();
            txbSearch.Clear();
        }

        private void MenuDelete_Click(object sender, RoutedEventArgs e)
        {
            List<Counterparties> list = dtgCounterparties.SelectedItems.Cast<Counterparties>().ToList();
            if (list.Count == 0)
            {
                MessageBox.Show($"Выберите контрагентов для удаления.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (list.Count == 1)
            {
                if (MessageBox.Show($"Вы точно хотите удалить запись №{list.First().idCounterparty}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        InvoicesOnArrival.UpdateAtIdCounterparty(list.First().idCounterparty);
                        ClassHelper.db.Counterparties.Remove(list.First());
                        ClassHelper.db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
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
                        foreach (Counterparties c in list)
                        {
                            InvoicesOnArrival.UpdateAtIdCounterparty(c.idCounterparty);
                            ClassHelper.db.Counterparties.Remove(c);
                        }
                        ClassHelper.db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else return;
            }
            MessageBox.Show("Данные удалены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            dtgCounterparties.ItemsSource = ClassHelper.db.Counterparties.ToList();
            CountItems.Text = dtgCounterparties.Items.Count.ToString();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageLobby());
        }
    }
}
