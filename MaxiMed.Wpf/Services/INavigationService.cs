using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MaxiMed.Wpf.Services
{
    public interface INavigationService
    {
        void SetFrame(System.Windows.Controls.Frame frame);
        void NavigateTo<TPage>() where TPage : System.Windows.Controls.Page;
        void Navigate(Page page);
    }
}
