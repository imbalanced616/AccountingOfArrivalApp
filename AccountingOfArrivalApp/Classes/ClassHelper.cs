using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace AccountingOfArrivalApp.Classes
{
    internal class ClassHelper
    {
        public static Frame frmObj;

        public static AccountingOfArrivalEntities db = new AccountingOfArrivalEntities();

        public static void GridColumnFastEdit(DataGridCell cell, RoutedEventArgs e)
        {
            if (cell == null || cell.IsEditing || cell.IsReadOnly) return;
            DataGrid dataGrid = FindVisualParent<DataGrid>(cell);
            if (dataGrid == null) return;
            if (!cell.IsFocused) cell.Focus();
            if (cell.Content is CheckBox)
            {
                if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow) if (!cell.IsSelected) cell.IsSelected = true;
                    else
                    {
                        DataGridRow row = FindVisualParent<DataGridRow>(cell);
                        if (row != null && !row.IsSelected) row.IsSelected = true;
                    }
            }
            else
            {
                if (cell.Content is ComboBox cb)
                {
                    dataGrid.BeginEdit(e);
                    cell.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                    cb.IsDropDownOpen = true;
                }
            }
        }

        private static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                if (parent is T correctlyTyped) return correctlyTyped;
                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }
    }
}
