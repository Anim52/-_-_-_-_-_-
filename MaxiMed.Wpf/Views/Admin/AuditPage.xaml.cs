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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MaxiMed.Wpf.Views.Admin
{
    /// <summary>
    /// Логика взаимодействия для AuditPage.xaml
    /// </summary>
    public partial class AuditPage : Page
    {
        public AuditPage(AuditViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            Loaded += async (_, __) => await vm.LoadAsync();
        }
    }
}
