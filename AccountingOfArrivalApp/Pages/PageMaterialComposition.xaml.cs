using AccountingOfArrivalApp.Classes;
using AccountingOfArrivalApp.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AccountingOfArrivalApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageMaterialComposition.xaml
    /// </summary>
    public partial class PageMaterialComposition : Page
    {
        private readonly Materials _currentItem = new Materials();
        public PageMaterialComposition(Materials selectedItem)
        {
            InitializeComponent();
            _currentItem = selectedItem;
            DataContext = _currentItem;

            LoadTreeView();
            btnBack.Click += BtnBackToNomenclature_Click;
        }

        public PageMaterialComposition(Storage selectedIdItem)
        {
            InitializeComponent();
            _currentItem = selectedIdItem.Material;
            DataContext = _currentItem;

            LoadTreeView();
            btnBack.Click += BtnBackToStorage_Click;
        }

        public void LoadTreeView()
        {
            foreach (Hierarchy h in Hierarchy.ToList().Where(x => x.IdParent == _currentItem.IdMaterial))
            {
                TreeViewItem item = new TreeViewItem() { Header = $"{h.MaterialChild.Name}                Кол-во: {h.Quantity}" };
                foreach (Hierarchy h2 in Hierarchy.ToList().Where(x => x.IdParent == h.MaterialChild.IdMaterial).ToList())
                    item.Items.Add(new TreeViewItem() { Header = $"{h2.MaterialChild.Name}                Кол-во: {h2.Quantity}" });
                trvMaterialComposition.Items.Add(item);
            }
        }

        private void BtnBackToNomenclature_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageNomenclature());
        }

        private void BtnBackToStorage_Click(object sender, RoutedEventArgs e)
        {
            ClassHelper.frmObj.Navigate(new PageStorage());
        }
    }
}
