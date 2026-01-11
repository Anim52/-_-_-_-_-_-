using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Audit;
using MaxiMed.Application.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Admin
{
    public partial class AuditViewModel : ObservableObject
    {
        private readonly IAuditService _audit;
        private readonly IServiceProvider _sp;

        public ObservableCollection<AuditLogDto> Items { get; } = new();
        [ObservableProperty] private AuditLogDto? selected;

        // Фильтры
        [ObservableProperty] private DateTime? fromDate;
        [ObservableProperty] private DateTime? toDate;
        [ObservableProperty] private string? actionText;
        [ObservableProperty] private string? entityText;
        [ObservableProperty] private string? searchText;
        [ObservableProperty] private int take = 300;

        // Пользователь фильтр (по желанию)
        public ObservableCollection<LookupItemDto> Users { get; } = new();
        [ObservableProperty] private LookupItemDto? selectedUser;

        [ObservableProperty] private string? errorText;
        [ObservableProperty] private bool isBusy;

        public AuditViewModel(IAuditService audit, IServiceProvider sp)
        {
            _audit = audit;
            _sp = sp;

            // дефолт: последние 7 дней
            ToDate = DateTime.Today.AddDays(1);
            FromDate = DateTime.Today.AddDays(-7);
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                ErrorText = null;

                Items.Clear();

                var filter = new AuditSearchFilter
                {
                    From = FromDate,
                    To = ToDate,
                    UserId = SelectedUser?.Id,
                    Action = ActionText,
                    Entity = EntityText,
                    Text = SearchText,
                    Take = Take
                };

                var list = await _audit.SearchAsync(filter);
                foreach (var x in list) Items.Add(x);
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
