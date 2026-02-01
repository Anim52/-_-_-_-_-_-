using MaxiMed.Wpf.ViewModels.Theme;
using System.Windows.Controls;

namespace MaxiMed.Wpf.Views.Theme;

public partial class SettingsPage : Page
{
    public SettingsPage(SettingsViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
