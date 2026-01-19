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
using System.Windows.Shapes;

namespace MaxiMed.Wpf.Views.Appointments
{
    /// <summary>
    /// Логика взаимодействия для AppointmentEditWindow.xaml
    /// </summary>
    public partial class AppointmentEditWindow : Window
    {
        public AppointmentEditWindow()
        {
            InitializeComponent();

            // следим за сменой DataContext и подписываемся на ViewModel
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is AppointmentEditViewModel oldVm)
                oldVm.RequestClose -= Vm_RequestClose;

            if (e.NewValue is AppointmentEditViewModel newVm)
                newVm.RequestClose += Vm_RequestClose;
        }

        private void Vm_RequestClose(bool ok)
        {
            // на всякий — через Dispatcher
            Dispatcher.Invoke(() =>
            {
                DialogResult = ok;
                Close();
            });
        }
    }
}
