using Windows.UI.Xaml.Controls;

namespace QrCodeGenerator.Pages.Content
{
    public sealed partial class TextContentPage : Page, IContentSettings
    {
        public TextContentPage()
        {
            this.InitializeComponent();
        }

        public IPreviewDisplay PreviewDisplay { get; set; }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            PreviewDisplay.DiplayString(ContentTextBox.Text);
        }
    }
}
