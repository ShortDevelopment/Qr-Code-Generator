using System;
using Windows.UI.Xaml.Controls;
using static QRCoder.PayloadGenerator;

namespace QrCodeGenerator.Pages.Content
{
    public sealed partial class UrlContentPage : Page, IContentSettings
    {
        public UrlContentPage()
        {
            this.InitializeComponent();
        }

        public IPreviewDisplay PreviewDisplay { get; set; }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if(Uri.TryCreate(ContentTextBox.Text, UriKind.Absolute, out var uri))
            {
                PreviewDisplay.DiplayPayload(new Url(uri.ToString()));
                PreviewWebView.Navigate(uri);
            }
        }
    }
}
