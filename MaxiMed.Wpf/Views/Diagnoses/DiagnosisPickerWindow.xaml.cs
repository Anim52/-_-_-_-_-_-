using MaxiMed.Application.Diagnoses;
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
    /// Логика взаимодействия для DiagnosisPickerWindow.xaml
    /// </summary>
    public partial class DiagnosisPickerWindow : Window
    {
        public DiagnosisDto? Result { get; private set; }

        public DiagnosisPickerWindow(DiagnosisPickerViewModel vm)
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
        {
            var vm = (DiagnosisPickerViewModel)DataContext;
            vm.PickCommand.Execute(null);
        }
    }
}
