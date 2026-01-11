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
using System.Windows.Shapes;

namespace MaxiMed.Wpf.Views.Branches
{
    /// <summary>
    /// Логика взаимодействия для BranchEditWindow.xaml
    /// </summary>
    public partial class BranchEditWindow : Window
    {
        public BranchEditWindow(BranchEditViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            vm.RequestClose += ok => { DialogResult = ok; Close(); };
        }
    }
}
