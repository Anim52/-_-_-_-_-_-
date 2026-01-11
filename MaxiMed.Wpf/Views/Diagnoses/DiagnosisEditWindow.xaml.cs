using MaxiMed.Wpf.ViewModels.Diagnoses;
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

namespace MaxiMed.Wpf.Views.Diagnoses
{
    /// <summary>
    /// Логика взаимодействия для DiagnosisEditWindow.xaml
    /// </summary>
    public partial class DiagnosisEditWindow : Window
    {
        public DiagnosisEditWindow(DiagnosisEditViewModel vm)
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
