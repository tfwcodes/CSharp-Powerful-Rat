using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;

namespace Client3
{
    public static class ScreenShot
    {
        public static void TakeScrenShot(string path)
        {
            var bp = new Bitmap(1920, 1080, PixelFormat.Format32bppArgb);

            var cr = Screen.AllScreens[0].Bounds;

            var graphics = Graphics.FromImage(bp);

            graphics.CopyFromScreen(cr.Top, cr.Left, 0, 0, cr.Size);
            bp.Save(path, ImageFormat.Jpeg);
        }

        
    }
}
    