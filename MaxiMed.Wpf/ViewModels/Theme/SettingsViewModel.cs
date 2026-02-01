using CommunityToolkit.Mvvm.ComponentModel;
using MaxiMed.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MaxiMed.Wpf.Services.ThemeService;

namespace MaxiMed.Wpf.ViewModels.Theme
{
    public sealed partial class SettingsViewModel : ObservableObject
    {
        public bool IsDarkTheme
        {
            get => UiOptions.IsDarkTheme;
            set
            {
                if (UiOptions.IsDarkTheme != value)
                {
                    UiOptions.IsDarkTheme = value;
                    OnPropertyChanged();
                    ThemeService.SetTheme(value);
                }
            }
        }

        public bool AnimationsEnabled
        {
            get => UiOptions.AnimationsEnabled;
            set
            {
                if (UiOptions.AnimationsEnabled != value)
                {
                    UiOptions.AnimationsEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> Accents { get; } =
            new() { "Blue", "Green", "Purple" };

        private string _selectedAccent = "Blue";
        public string SelectedAccent
        {
            get => _selectedAccent;
            set
            {
                if (SetProperty(ref _selectedAccent, value))
                    ThemeService.SetAccent($"Theme.Accent.{value}.xaml");
            }
        }
    }

}
