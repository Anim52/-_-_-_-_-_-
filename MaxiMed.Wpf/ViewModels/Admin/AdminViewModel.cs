using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Wpf.Services;
using MaxiMed.Wpf.Views.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Admin
{
    public partial class AdminViewModel : ObservableObject
    {
        private readonly INavigationService _nav;

        public AdminViewModel(INavigationService nav) => _nav = nav;

        [RelayCommand]
        private void OpenUsers() => _nav.NavigateTo<UserListPage>();

        [RelayCommand]
        private void OpenAudit() => _nav.NavigateTo<AuditPage>();

    }
}
