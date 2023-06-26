using AccountingOfArrivalApp.Classes;
using AccountingOfArrivalApp.Pages.Add_Edit;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Word = Microsoft.Office.Interop.Word;

namespace AccountingOfArrivalApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageArrivals.xaml
    /// </summary>
    public partial class PageArrivals : System.Windows.Controls.Page
    {
        public PageArrivals()
        {
            InitializeComponent();
            dtgArrivals.ItemsSource = ClassHelper.db.InvoicesOnArrival.ToList();
            CountItems.Text = dtgArrivals.Items.Count.ToString();
            if (User.CurrentUser.IdTypeUser == 1 || User.CurrentUser.IdTypeUser == 3)
            {
                dtgArrivals.Columns.Add(new DataGridCheckBoxColumn() { Header = "Распре-\nделено", HeaderStyle= (System.Windows.Style)FindResource("dgshStyle"), Binding = new Binding("DistributedInvoice") });
                MenuItem menuItem = new MenuItem() { Header = "Распределить", Icon = FindResource("icoDistribute") };
                menuItem.Click += (s, e) => MenuDistribute_Click(s, e);
                dtgArrivals.ContextMenu.Items.Insert(4, menuItem);
                dtgArrivals.ContextMenu.Items.Insert(5, new Separator() { Margin = new Thickness(0,3,0,3) });
            }
        }

        private void TxbSearch_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txbSearch.Focus();
        }

        private void TxbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbSearch.Text.Count() != 0) dtgArrivals.ItemsSource = InvoicesOnArrival.ToListBySearch(txbSearch.Text.ToLower());
            else dtgArrivals.ItemsSource = InvoicesOnArrival.ToList();
            CountItems.Text = dtgArrivals.Items.Count.ToString();
        }

        private void MenuAddOrEdit_Click(object sender, RoutedEventArgs e)
        {
            if (((MenuItem)sender).Header.ToString() == "Добавить") ClassHelper.frmObj.Navigate(new AddEditPageArrival(null));
            else
            {
                if (dtgArrivals.SelectedItem is InvoicesOnArrival invoice)
                    if (invoice.DistributedInvoice == true) MessageBox.Show($"Поставка №{invoice.idInvoice} уже распределена и не может быть изменена.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else ClassHelper.frmObj.Navigate(new AddEditPageArrival(invoice));
                else MessageBox.Show("Выберите поставку для изменения.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void MenuUpdate_Click(object sender, RoutedEventArgs e)
        {
            dtgArrivals.ItemsSource = ClassHelper.db.InvoicesOnArrival.ToList();
            CountItems.Text = dtgArrivals.Items.Count.ToString();
            txbSearch.Clear();
        }

        private void MenuDistribute_Click(object sender, RoutedEventArgs e)
        {
            if (dtgArrivals.SelectedItem is InvoicesOnArrival invoice)
                if (invoice.DistributedInvoice == true) MessageBox.Show($"Поставка №{invoice.idInvoice} уже распределена и не может быть распределена снова.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                else ClassHelper.frmObj.Navigate(new AddEditMovementDistribution(invoice));
            else MessageBox.Show("Выберите поставку для распределения.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void MenuExportToWord_Click(object sender, RoutedEventArgs e)
        {
            if (dtgArrivals.SelectedItem is InvoicesOnArrival invoice)
            {
                Word.Application app = null;
                List<CompositionsOfInvoice> printItems = invoice.CompositionsOfInvoice.ToList();
                try
                {
                    app = new Word.Application();
                    app.Documents.Add($"{Directory.GetCurrentDirectory().Split(new string[] { "\\bin" }, StringSplitOptions.None)[0]}\\Resources\\Приходная накладная.docx");
                    Dictionary<string, string> items = new Dictionary<string, string>
                    {
                        { "<№>", invoice.idInvoice.ToString() },
                        { "<DATE_NOW>", DateTime.Now.ToString("«dd»   MMMM   yyyy", CultureInfo.CreateSpecificCulture("ru-RU")) },
                        { "<COUNTERPARTY>", invoice.Counterparties?.Name },
                        { "<INN>", invoice.Counterparties?.INN },
                        { "<ADDRESS>", invoice.Counterparties?.Address },
                        { "<MANAGER>", invoice.Users.FIO }
                    };
                    foreach (var item in items)
                    {
                        Find find = app.Selection.Find;
                        find.Text = item.Key;
                        find.Replacement.Text = item.Value;
                        find.Execute(FindText: Type.Missing, MatchCase: false, MatchWholeWord: false, MatchWildcards: false, MatchSoundsLike: Type.Missing, MatchAllWordForms: false, Forward: true, Wrap: Word.WdFindWrap.wdFindContinue, Format: false, ReplaceWith: Type.Missing, Replace: Word.WdReplace.wdReplaceAll);
                    }
                    Table table = app.ActiveDocument.Tables.Add(app.ActiveDocument.Paragraphs.Add().Range, printItems.Count() + 1, 5);
                    table.Borders.InsideLineStyle = table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Range.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    table.Rows[1].Range.Bold = 1;
                    table.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    table.Range.Font.Size = 10;
                    table.Range.Font.Name = "Arial";

                    table.Cell(1, 1).Range.Text = "№";
                    table.Range.Columns[1].Width = 30;

                    table.Cell(1, 2).Range.Text = "Материал";
                    table.Range.Columns[2].Width = 215;

                    table.Cell(1, 3).Range.Text = "Кол-во";
                    table.Range.Columns[3].Width = 80;

                    table.Cell(1, 4).Range.Text = "Цена, руб.";
                    table.Range.Columns[4].Width = 80;

                    table.Cell(1, 5).Range.Text = "Сумма, руб.";
                    table.Range.Columns[5].Width = 80;

                    for (int i = 0; i < printItems.Count; i++)
                    {
                        table.Cell(i + 2, 1).Range.Text = (i + 1).ToString();
                        table.Cell(i + 2, 2).Range.Text = printItems[i].Materials?.Name;
                        table.Cell(i + 2, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        table.Cell(i + 2, 3).Range.Text = printItems[i].Quantity.ToString();
                        table.Cell(i + 2, 4).Range.Text = printItems[i].PricePerUnit.ToString();
                        table.Cell(i + 2, 5).Range.Text = printItems[i].Amount.ToString();
                    }

                    Range range = app.ActiveDocument.Paragraphs.Add().Range;
                    range.Text = $"\nСумма поставки: {invoice.Amount} руб.\n\nОтпустил ______________/______________              Получил ______________/______________";
                    range.Font.Size = 10.5f;
                    range.Font.Name = "Arial";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (app != null) app.Visible = true;
            }
            else MessageBox.Show("Выберите поставку для экспорта в Word.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void MenuDelete_Click(object sender, RoutedEventArgs e)
        {
            List<InvoicesOnArrival> list = dtgArrivals.SelectedItems.Cast<InvoicesOnArrival>().ToList();
            if (list.Count == 0)
            {
                MessageBox.Show($"Выберите поставки для удаления.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (list.Count == 1)
            {
                if (MessageBox.Show($"Вы точно хотите удалить запись №{list.First().idInvoice}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        InvoicesOnArrival i = list.First();
                        foreach (CompositionsOfInvoice c in ClassHelper.db.CompositionsOfInvoice.Where(x => x.idInvoice == i.idInvoice))
                            ClassHelper.db.CompositionsOfInvoice.Remove(c);
                        ClassHelper.db.InvoicesOnArrival.Remove(i);
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
                        foreach (InvoicesOnArrival i in list)
                        {
                            foreach (CompositionsOfInvoice c in ClassHelper.db.CompositionsOfInvoice.Where(x => x.idInvoice == i.idInvoice))
                                ClassHelper.db.CompositionsOfInvoice.Remove(c);
                            ClassHelper.db.InvoicesOnArrival.Remove(i);
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
            dtgArrivals.ItemsSource = ClassHelper.db.InvoicesOnArrival.ToList();
            CountItems.Text = dtgArrivals.Items.Count.ToString();
        }

        private void DtgArrivals_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dtgArrivals.SelectedItem != null) ClassHelper.frmObj.Navigate(new PageArrivalComposition((InvoicesOnArrival)dtgArrivals.SelectedItem));
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageLobby());
        }
    }
}
