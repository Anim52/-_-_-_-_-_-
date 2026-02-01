using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Wpf.Api;
using MaxiMed.Wpf.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Theme
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly UiOptions _opts;
        private readonly ApiClient _api;
        private readonly IConfiguration _cfg;

        public SettingsViewModel(ApiClient api, IConfiguration cfg)
        {
            _api = api;
            _cfg = cfg;
            _opts = ThemeService.Options;

            // init selections
            _isDarkTheme = _opts.IsDarkTheme;
            _animationsEnabled = _opts.AnimationsEnabled;
            _selectedAccent = _opts.Accent;
            _selectedDensity = _opts.Density;
            _baseFontSize = _opts.BaseFontSize;
            _selectedCornerRadius = _opts.CornerRadius;
            _confirmDestructiveActions = _opts.ConfirmDestructiveActions;
            _rememberLastPage = _opts.RememberLastPage;
            _selectedStartPage = _opts.StartPage;
            _selectedAnimationSpeed = _opts.AnimationSpeed;

            ApiBaseUrl = (_cfg["Api:BaseUrl"] ?? "http://localhost:5087/").Trim();
        }

        // ===== Options lists =====
        public ObservableCollection<string> Accents { get; } = new() { "Blue", "Green", "Purple" };
        public ObservableCollection<string> Densities { get; } = new() { "Compact", "Normal", "Comfort" };
        public ObservableCollection<string> CornerRadii { get; } = new() { "Sharp", "Rounded", "ExtraRounded" };
        public ObservableCollection<string> AnimationSpeeds { get; } = new() { "Fast", "Normal", "Slow" };
        public ObservableCollection<string> StartPages { get; } = new()
        {
            "Patients","Appointments","Schedule","WeekSchedule","FreeSlots","Reports","Admin","Settings"
        };

        // ===== Bindable properties =====
        private bool _isDarkTheme;
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (SetProperty(ref _isDarkTheme, value))
                {
                    _opts.IsDarkTheme = value;
                    SaveAndApply();
                }
            }
        }

        private bool _animationsEnabled = true;
        public bool AnimationsEnabled
        {
            get => _animationsEnabled;
            set
            {
                if (SetProperty(ref _animationsEnabled, value))
                {
                    _opts.AnimationsEnabled = value;
                    _opts.Save();
                }
            }
        }

        private string _selectedAccent = "Blue";
        public string SelectedAccent
        {
            get => _selectedAccent;
            set
            {
                if (SetProperty(ref _selectedAccent, value))
                {
                    _opts.Accent = value;
                    SaveAndApply();
                }
            }
        }

        private string _selectedDensity = "Normal";
        public string SelectedDensity
        {
            get => _selectedDensity;
            set
            {
                if (SetProperty(ref _selectedDensity, value))
                {
                    _opts.Density = value;
                    SaveAndApply();
                }
            }
        }

        private double _baseFontSize = 13;
        public double BaseFontSize
        {
            get => _baseFontSize;
            set
            {
                if (SetProperty(ref _baseFontSize, value))
                {
                    _opts.BaseFontSize = value;
                    SaveAndApply();
                }
            }
        }

        private string _selectedCornerRadius = "Rounded";
        public string SelectedCornerRadius
        {
            get => _selectedCornerRadius;
            set
            {
                if (SetProperty(ref _selectedCornerRadius, value))
                {
                    _opts.CornerRadius = value;
                    SaveAndApply();
                }
            }
        }

        private bool _confirmDestructiveActions = true;
        public bool ConfirmDestructiveActions
        {
            get => _confirmDestructiveActions;
            set
            {
                if (SetProperty(ref _confirmDestructiveActions, value))
                {
                    _opts.ConfirmDestructiveActions = value;
                    _opts.Save();
                }
            }
        }

        private bool _rememberLastPage = true;
        public bool RememberLastPage
        {
            get => _rememberLastPage;
            set
            {
                if (SetProperty(ref _rememberLastPage, value))
                {
                    _opts.RememberLastPage = value;
                    _opts.Save();
                }
            }
        }

        private string _selectedStartPage = "Patients";
        public string SelectedStartPage
        {
            get => _selectedStartPage;
            set
            {
                if (SetProperty(ref _selectedStartPage, value))
                {
                    _opts.StartPage = value;
                    _opts.Save();
                }
            }
        }

        private string _selectedAnimationSpeed = "Normal";
        public string SelectedAnimationSpeed
        {
            get => _selectedAnimationSpeed;
            set
            {
                if (SetProperty(ref _selectedAnimationSpeed, value))
                {
                    _opts.AnimationSpeed = value;
                    SaveAndApply();
                }
            }
        }

        public string ApiBaseUrl { get; }
        private string _apiStatus = "не проверено";
        public string ApiStatus { get => _apiStatus; set => SetProperty(ref _apiStatus, value); }

        private bool _isChecking;
        public bool IsChecking { get => _isChecking; set => SetProperty(ref _isChecking, value); }

        [RelayCommand]
        private async Task CheckApiAsync()
        {
            if (IsChecking) return;
            try
            {
                IsChecking = true;
                var sw = Stopwatch.StartNew();

                // try GET "/" - any response is considered "reachable"
                var ok = await _api.TryPingAsync("/");
                sw.Stop();

                ApiStatus = ok
                    ? $"online ({sw.ElapsedMilliseconds} ms)"
                    : "offline";
            }
            catch
            {
                ApiStatus = "offline";
            }
            finally
            {
                IsChecking = false;
            }
        }

        private void SaveAndApply()
        {
            _opts.Save();
            ThemeService.ApplyFromOptions(_opts);
        }
    }
}
