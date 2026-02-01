using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Auth;
using MaxiMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.WebRequestMethods;

namespace MaxiMed.Wpf.ViewModels.Auth
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _auth;

        public event Action<User>? LoginSucceeded;

        [ObservableProperty] private string login = "";
        [ObservableProperty] private string errorText = "";
        [ObservableProperty] private bool isBusy;

        public LoginViewModel(IAuthService auth) => _auth = auth;
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorText);

        partial void OnErrorTextChanged(string value)
            => OnPropertyChanged(nameof(HasError));


        [RelayCommand]
        private async Task LoginAsync(string password)
        {
            MessageBox.Show("OnLoginClick fired");


            if (IsBusy) return;

            try
            {
                IsBusy = true;
                ErrorText = "";

                if (string.IsNullOrWhiteSpace(Login))
                {
                    ErrorText = "Введите логин";
                    return;
                }
                if (string.IsNullOrWhiteSpace(password))
                {
                    ErrorText = "Введите пароль";
                    return;
                }

                var user = await _auth.LoginAsync(Login.Trim(), password);
                if (user is null)
                {
                    ErrorText = "Неверный логин или пароль";
                    return;
                }

                LoginSucceeded?.Invoke(user);
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
