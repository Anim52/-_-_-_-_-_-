using MaxiMed.Domain.Entities;
using MaxiMed.Wpf.ViewModels;
using MaxiMed.Wpf.Views;
using MaxiMed.Wpf.Views.Auth;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<bool> LoginAndShowMainAsync()
        {
            // показываем логин
            var login = _sp.GetRequiredService<LoginWindow>();
            var ok = login.ShowDialog() == true;
            if (!ok) return false;

            var user = (User)login.Tag!;
            _session.SetUser(user);

            // создаём НОВОЕ главное окно (Transient)
            var main = _sp.GetRequiredService<MainWindow>();
            System.Windows.Application.Current.MainWindow = main;

            if (main.DataContext is MainWindowViewModel mvm)
                mvm.RefreshPermissions();

            main.Show();
            main.Activate();

            return true;
        }

        public  Task LogoutToLoginAsync()
        {
            // 1) чистим сессию
            _session.Clear();

            // 2) закрываем текущее главное окно (но НЕ делаем Shutdown!)
            var oldMain = System.Windows.Application.Current.MainWindow;
            oldMain?.Hide();   // вместо Close, чтобы не ловить "после закрытия"
                               // oldMain?.Close(); // можно, но тогда надо следить за shutdownmode

            // 3) показываем логин
            var login = _sp.GetRequiredService<LoginWindow>();
            var ok = login.ShowDialog() == true;

            if (!ok)
            {
                // пользователь нажал "Отмена" — тогда выходим из приложения
                System.Windows.Application.Current.Shutdown();
                return Task.CompletedTask;
            }

            var user = (User)login.Tag;
            _session.SetUser(user);

            // 4) открываем новое главное окно
            var main = _sp.GetRequiredService<MainWindow>();
            System.Windows.Application.Current.MainWindow = main;
            main.Show();
            main.Activate();

            // 5) обновляем права/кнопки
            if (main.DataContext is MainWindowViewModel mvm)
                mvm.RefreshPermissions();

            // 6) старое окно закрываем, если оно было
            if (oldMain is not null)
                oldMain.Close();

            return Task.CompletedTask;
        }
    }
}

