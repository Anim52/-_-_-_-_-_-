using MaxiMed.Domain.Entities;
using MaxiMed.Wpf.ViewModels;
using MaxiMed.Wpf.ViewModels.Auth;
using MaxiMed.Wpf.Views;
using MaxiMed.Wpf.Views.Auth;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace MaxiMed.Wpf.Services
{
    public sealed class AuthFlowService : IAuthFlowService
    {
        private readonly IServiceProvider _sp;
        private readonly ISessionService _session;

        public AuthFlowService(IServiceProvider sp, ISessionService session)
        {
            _sp = sp;
            _session = session;
        }

        public Task<bool> LoginAndShowMainAsync()
        {
            // 1) логин через VM + DialogResult
            var user = ShowLoginDialogAndGetUser();
            if (user is null) return Task.FromResult(false);

            _session.SetUser(user);

            // 2) новое главное окно
            var main = _sp.GetRequiredService<MainWindow>();
            System.Windows.Application.Current.MainWindow = main;

            if (main.DataContext is MainWindowViewModel mvm)
            {
                mvm.RefreshPermissions();
                mvm.NavigateInitial();
            }

            main.Show();

            main.Activate();

            return Task.FromResult(true);
        }

        public Task LogoutToLoginAsync()
        {
            _session.Clear();

            var oldMain = System.Windows.Application.Current.MainWindow;
            oldMain?.Hide();

            var user = ShowLoginDialogAndGetUser();
            if (user is null)
            {
                System.Windows.Application.Current.Shutdown();
                return Task.CompletedTask;
            }

            _session.SetUser(user);

            var main = _sp.GetRequiredService<MainWindow>();
            System.Windows.Application.Current.MainWindow = main;

            if (main.DataContext is MainWindowViewModel mvm)
                mvm.RefreshPermissions();

            main.Show();
            main.Activate();

            if (oldMain is not null)
                oldMain.Close();

            return Task.CompletedTask;
        }

        private User? ShowLoginDialogAndGetUser()
        {
            var loginWindow = _sp.GetRequiredService<LoginWindow>();
            var loginVm = _sp.GetRequiredService<LoginViewModel>();

            loginWindow.DataContext = loginVm;

            User? resultUser = null;

            // важно: подписка ДО ShowDialog
            loginVm.LoginSucceeded += user =>
            {
                resultUser = user;
                loginWindow.DialogResult = true; // завершит ShowDialog()
                loginWindow.Close();
            };

            // пользователь нажал “Отмена” (или закрыл окно)
            var ok = loginWindow.ShowDialog() == true;

            // на всякий случай отписаться (чтобы не копились подписки)
            loginVm.LoginSucceeded -= user =>
            {
                resultUser = user;
                loginWindow.DialogResult = true;
                loginWindow.Close();
            };

            return ok ? resultUser : null;
        }
    }
}
