using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MaxiMed.Wpf.Services
{
    public sealed class NavigationService : INavigationService
    {
        private readonly IServiceProvider _sp;
        private Frame? _frame;

        public NavigationService(IServiceProvider sp) => _sp = sp;

        public void SetFrame(Frame frame) => _frame = frame;

        public void NavigateTo<TPage>() where TPage : Page
        {
            if (_frame is null) throw new InvalidOperationException("Frame не задан. Вызови SetFrame()");
            var page = _sp.GetRequiredService<TPage>();
            _frame.Navigate(page);
        }
        public void Navigate(Page page)
        {
            _frame?.Navigate(page);
        }
    }
}
