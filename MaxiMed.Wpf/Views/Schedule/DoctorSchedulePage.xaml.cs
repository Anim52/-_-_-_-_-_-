using MaxiMed.Wpf.ViewModels.Schedule;
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

namespace MaxiMed.Wpf.Views.Schedule
{
    /// <summary>
    /// Логика взаимодействия для DoctorSchedulePage.xaml
    /// </summary>
    public partial class DoctorSchedulePage : Page
    {
        public DoctorSchedulePage(DoctorScheduleViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            Loaded += async (_, __) => await vm.InitAsync();
        }
    }
}
