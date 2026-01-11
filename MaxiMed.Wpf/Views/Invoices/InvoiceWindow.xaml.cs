using MaxiMed.Wpf.ViewModels.Invoices;
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

namespace MaxiMed.Wpf.Views.Invoices
{
    /// <summary>
    /// Логика взаимодействия для InvoiceWindow.xaml
    /// </summary>
    public partial class InvoiceWindow : Window
    {
        public InvoiceWindow(InvoiceViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

          
        }
    }
}
