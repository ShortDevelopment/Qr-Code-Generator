using QrCodeGenerator.Pages.Content;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
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

        private async Task RenderToStream(IRandomAccessStream stream)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(PreviewBorder);

            byte[] pixels = (await bitmap.GetPixelsAsync()).ToArray();

            const int dpi = 150; // 96;

            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, dpi, dpi, pixels);
            await encoder.FlushAsync();

            stream.Seek(0);
        }

        private async void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            IRandomAccessStream stream = new InMemoryRandomAccessStream();
            await RenderToStream(stream);

            DataPackage dataPackage = new DataPackage();
            dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));
            Clipboard.SetContent(dataPackage);

            Flyout flyout = new Flyout();
            flyout.Placement = FlyoutPlacementMode.Bottom;
            flyout.ShowMode = FlyoutShowMode.Transient;
            flyout.Content = new TextBlock() { Text = "Copied!" };
            flyout.ShowAt(sender as FrameworkElement);

            await Task.Delay(1000);

            flyout.Hide();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeChoices.Add("Image", new List<string>() { ".png" });

            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    await RenderToStream(stream);
        }
    }
}
