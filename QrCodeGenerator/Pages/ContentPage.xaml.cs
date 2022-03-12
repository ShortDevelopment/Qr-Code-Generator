using QrCodeGenerator.Pages.Content;
using QRCoder;
using System;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace QrCodeGenerator.Pages
{
    public sealed partial class ContentPage : Page, IPreviewDisplay
    {
        public ContentPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Page content;
            switch (e.Parameter as string)
            {
                case "url":
                    content = new UrlContentPage();
                    break;
                default:
                    content = new TextContentPage();
                    break;
            }
            IContentSettings settings = content as IContentSettings;
            settings.PreviewDisplay = this;
            ContentFrame.Content = content;
        }

        #region IPreviewDisplay
        public void DiplayPayload(PayloadGenerator.Payload payload, QRCodeGenerator.ECCLevel level = QRCodeGenerator.ECCLevel.Q)
            => DiplayString(payload.ToString(), level);

        public async void DiplayString(string content, QRCodeGenerator.ECCLevel level = QRCodeGenerator.ECCLevel.Q)
        {
            using (QRCodeGenerator generator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = generator.CreateQrCode(content, level))
            using (BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData))
            {
                byte[] qrCodeImage = qrCode.GetGraphic(20);
                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                    {
                        writer.WriteBytes(qrCodeImage);
                        await writer.StoreAsync();
                    }
                    var image = new BitmapImage();
                    await image.SetSourceAsync(stream);

                    PreviewImage.Source = image;
                }
            }
        }
        #endregion
    }
}
