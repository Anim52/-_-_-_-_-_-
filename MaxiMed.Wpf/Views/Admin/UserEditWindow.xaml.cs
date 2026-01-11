using MaxiMed.Wpf.ViewModels.Admin;
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

namespace MaxiMed.Wpf.Views.Admin
{
    /// <summary>
    /// Логика взаимодействия для UserEditWindow.xaml
    /// </summary>
    public partial class UserEditWindow : Window
    {
        public UserEditWindow(UserEditViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            // PasswordBox не биндится нормально — руками
            Pwd.PasswordChanged += (_, __) => vm.NewPassword = Pwd.Password;

            vm.RequestClose += ok =>
            {
                DialogResult = ok;
                Close();
            };
        }
        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            var vm = (UserEditViewModel)DataContext;
            vm.NewPassword = string.IsNullOrWhiteSpace(Pwd.Password) ? null : Pwd.Password;
            vm.SaveCommand.Execute(null);
        }

    }
}
