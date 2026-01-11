using MaxiMed.Wpf.ViewModels.Branches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MaxiMed.Wpf.Views.Branches
{
    /// <summary>
    /// Логика взаимодействия для BranchesPage.xaml
    /// </summary>
    public partial class BranchesPage : Page
    {
        public BranchesPage(BranchesViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            Loaded += async (_, __) => await vm.LoadAsync();
        }
    }
}
