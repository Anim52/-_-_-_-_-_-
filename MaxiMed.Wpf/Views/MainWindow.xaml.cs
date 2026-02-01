using MaxiMed.Wpf.Services;
using MaxiMed.Wpf.ViewModels;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace MaxiMed.Wpf.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel vm, INavigationService nav)
        {
            InitializeComponent();
            DataContext = vm;
            nav.SetFrame(MainFrame);
            MainFrame.Navigated += (_, __) => PlayPageTransition();
        }
        private void PlayPageTransition()
        {
            if (!ThemeService.Options.AnimationsEnabled)
            {
                ContentHost.Opacity = 1;
                return;
            }

            var sb = new Storyboard();

            var fade = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(160)
            };
            Storyboard.SetTarget(fade, ContentHost);
            Storyboard.SetTargetProperty(fade, new PropertyPath("Opacity"));

            var slide = new DoubleAnimation
            {
                From = 8,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(160)
            };
            Storyboard.SetTarget(slide, ContentHost);
            Storyboard.SetTargetProperty(slide, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            sb.Children.Add(fade);
            sb.Children.Add(slide);

            ContentHost.Opacity = 0;
            sb.Begin();
        }
    }
}
