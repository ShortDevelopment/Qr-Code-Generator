using System;
using Windows.UI.Xaml.Controls;
using static QRCoder.PayloadGenerator;

namespace QrCodeGenerator.Pages.Content
{
    public sealed partial class WiFiContentPage : Page, IContentSettings
    {
        public WiFiContentPage()
        {
            this.InitializeComponent();

            ModeComboBox.ItemsSource = Enum.GetNames(typeof(WiFi.Authentication));
        }

        public IPreviewDisplay PreviewDisplay { get; set; }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (ModeComboBox.SelectedItem == null)
                return;
            var mode = Enum.Parse<WiFi.Authentication>(ModeComboBox.SelectedItem as string);
            PreviewDisplay.DiplayPayload(new WiFi(SsidTextBox.Text, PwdTextBox.Text, mode));
        }
    }
}
