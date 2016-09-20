using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace CoreLibrary.Utils.ScreenshotManager
{
    public class ScreenshotManager : IScreenshotManager
    {
        private const string Extension = ".png";
        private readonly IStorageFolder _mapScreensFolder;

        public ScreenshotManager(IStorageFolder mapScreensFolder)
        {
            _mapScreensFolder = mapScreensFolder;
        }

        public async Task<BitmapImage> OpenScreenFromPathAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
                return await Task.FromResult<BitmapImage>(null).ConfigureAwait(true);

            var imageFile = await _mapScreensFolder.GetFileAsync(path);

            var bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(await imageFile.OpenReadAsync());
            return bitmap;
        }

        public async Task<string> TakeScreenshotAsync(UIElement element, DisplayInformation displayInformation, string fileName)
        {
            var dpi = displayInformation.LogicalDpi;
            var imageFile = await OpenOrCrateFileAsync(fileName + Extension).ConfigureAwait(true);

            using (var randomAccessStream = await imageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var renderTargetBitmap = new RenderTargetBitmap();
                await renderTargetBitmap.RenderAsync(element);
                var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();

                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, randomAccessStream);
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)renderTargetBitmap.PixelWidth,
                    (uint)renderTargetBitmap.PixelHeight,
                    dpi,
                    dpi,
                    pixelBuffer.ToArray());

                await encoder.FlushAsync();
            }

            return fileName + Extension;
        }

        private async Task<IStorageFile> OpenOrCrateFileAsync(string path)
        {
            try
            {
                return await _mapScreensFolder.GetFileAsync(path);
            }
            catch (FileNotFoundException)
            {
                return await _mapScreensFolder.CreateFileAsync(path);
            }
        }
    }
}