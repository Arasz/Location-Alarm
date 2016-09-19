using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace CoreLibrary.Utils.ScreenshotManager
{
    /// <summary>
    /// Can take screenshots of UIElements, and load them from storage 
    /// </summary>
    public interface IScreenshotManager
    {
        Task<BitmapImage> OpenScreenFromPathAsync(string path);

        Task<string> TakeScreenshotAsync(UIElement element, DisplayInformation displayInformation, string fileName);
    }
}