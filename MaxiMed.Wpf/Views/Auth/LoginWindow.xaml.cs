using CommunityToolkit.Mvvm.Input;
using MaxiMed.Domain.Entities;
using MaxiMed.Wpf.ViewModels.Auth;
using MaxiMed.Wpf.Services;
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


namespace MaxiMed.Wpf.Views.Auth
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool _passwordVisible;

        public LoginWindow()
        {
            InitializeComponent();

            Loaded += (_, __) =>
            {
                if (!ThemeService.Options.AnimationsEnabled) return;

                Card.Opacity = 0;
                ((System.Windows.Media.TranslateTransform)Card.RenderTransform).Y = 10;

                var sb = new Storyboard();

                var fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(180));
                Storyboard.SetTarget(fade, Card);
                Storyboard.SetTargetProperty(fade, new PropertyPath("Opacity"));

                var slide = new DoubleAnimation(10, 0, TimeSpan.FromMilliseconds(180));
                Storyboard.SetTarget(slide, Card);
                Storyboard.SetTargetProperty(slide,
                    new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

                sb.Children.Add(fade);
                sb.Children.Add(slide);
                sb.Begin();
            };
        }

        private void OnTogglePassword(object sender, RoutedEventArgs e)
        {
            _passwordVisible = !_passwordVisible;

            if (_passwordVisible)
            {
                PwdText.Text = Pwd.Password;
                PwdWrap.Visibility = Visibility.Collapsed;
                PwdTextWrap.Visibility = Visibility.Visible;
                PwdText.Focus();
                PwdText.CaretIndex = PwdText.Text.Length;
            }
            else
            {
                Pwd.Password = PwdText.Text;
                PwdTextWrap.Visibility = Visibility.Collapsed;
                PwdWrap.Visibility = Visibility.Visible;
                Pwd.Focus();
            }
        }

        private void OnLoginClick(object sender, RoutedEventArgs e)
        {
            var password = PwdWrap.Visibility == Visibility.Visible
                ? Pwd.Password
                : PwdText.Text;

            if (DataContext is not LoginViewModel vm)
                return;

            if (vm.LoginCommand.CanExecute(password))
                vm.LoginCommand.Execute(password);
        }

        private void OnCancelClick(object sender, RoutedEventArgs e) => Close();

    }
}

