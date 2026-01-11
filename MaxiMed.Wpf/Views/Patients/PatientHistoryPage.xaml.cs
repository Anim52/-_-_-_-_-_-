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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MaxiMed.Wpf.Views.Patients
{
    /// <summary>
    /// Логика взаимодействия для PatientHistoryPage.xaml
    /// </summary>
    public partial class PatientHistoryPage : Page
    {
        public PatientHistoryPage(PatientHistoryViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
