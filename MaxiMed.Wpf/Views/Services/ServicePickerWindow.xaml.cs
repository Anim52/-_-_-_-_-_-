using MaxiMed.Application.Services;
using MaxiMed.Wpf.ViewModels.Services;
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

namespace MaxiMed.Wpf.Views.Services
{
    /// <summary>
    /// Логика взаимодействия для ServicePickerWindow.xaml
    /// </summary>
    public partial class ServicePickerWindow : Window
    {
        public ServiceDto? Result { get; private set; }

        public ServicePickerWindow(ServicePickerViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            vm.Picked += dto =>
            {
                Result = dto;
                DialogResult = true;
                Close();
            };

            Loaded += async (_, __) => await vm.SearchAsync();
        }

        private void OnDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
            => ((ServicePickerViewModel)DataContext).PickCommand.Execute(null);
    }
}
