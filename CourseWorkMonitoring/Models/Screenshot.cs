using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CourseWorkMonitoring.Models
{
    public class Screenshot
    {
        public void TakeScreenshot(string saveDirectory)
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            using (Bitmap bitmap = new Bitmap(screenWidth, screenHeight))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight));
                }

                if (!Directory.Exists(saveDirectory))
                {
                    Directory.CreateDirectory(saveDirectory);
                }

                string fileName = Path.Combine(saveDirectory, $"Screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                bitmap.Save(fileName, ImageFormat.Png);
            }
        }
    }
}
