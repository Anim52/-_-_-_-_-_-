using MaxiMed.Wpf.ViewModels.Visits;
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

namespace MaxiMed.Wpf.Views.Visits
{
    /// <summary>
    /// Логика взаимодействия для VisitEditWindow.xaml
    /// </summary>
    public partial class VisitEditWindow : Window
    {
        public VisitEditWindow(VisitEditViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            vm.RequestClose += ok =>
            {
                DialogResult = ok;
                Close();
            };
        }
    }
}
