using MaxiMed.Domain.Entities;
using MaxiMed.Wpf.ViewModels.Auth;
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

namespace MaxiMed.Wpf.Views.Auth
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow(LoginViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            vm.LoginSucceeded += (User user) =>
            {
                Tag = user;          
                DialogResult = true; 
                Close();
            };
        }

        private void OnLoginClick(object sender, RoutedEventArgs e)
        {
            var vm = (LoginViewModel)DataContext;
            vm.LoginCommand.Execute(Pwd.Password);
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
