using System;
using Windows.UI.Xaml.Controls;

namespace QrCodeGenerator.Pages
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        public string UI_CurrentYear { get; } = DateTime.Now.ToString("yyyy");
        public Version UI_VersionInfo { get; } = typeof(Program).Assembly.GetName().Version;
    }
}