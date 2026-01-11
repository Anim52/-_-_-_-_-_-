using MaxiMed.Wpf.ViewModels.Appointments;
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

namespace MaxiMed.Wpf.Views.Appointments
{
    /// <summary>
    /// Логика взаимодействия для AppointmentsPage.xaml
    /// </summary>
    public partial class AppointmentsPage : Page
    {
        public AppointmentsPage(AppointmentsViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            Loaded += async (_, __) => await vm.InitAsync();
        }
    }
}
