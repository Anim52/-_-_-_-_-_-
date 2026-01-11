using MaxiMed.Wpf.ViewModels.Patients;
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

namespace MaxiMed.Wpf.Views.Patients
{
    /// <summary>
    /// Логика взаимодействия для PatientEditWindow.xaml
    /// </summary>
    public partial class PatientEditWindow : Window
    {
        public PatientEditWindow(PatientEditViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            vm.RequestClose += OnRequestClose;
        }

        private void OnRequestClose(bool dialogResult)
        {
            DialogResult = dialogResult;
            Close();
        }
    }
}
