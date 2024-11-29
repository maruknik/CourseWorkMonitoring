using AForge.Video.DirectShow;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


namespace CourseWorkMonitoring.Models
{
    public class Webcam
    {
        private VideoCaptureDevice videoSource;

        public void CapturePhoto(string saveDirectory)
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += (sender, eventArgs) =>
            {
                using (var frame = (Bitmap)eventArgs.Frame.Clone())
                {
                    string fileName = Path.Combine(saveDirectory, $"Photo_{DateTime.Now:yyyyMMdd_HHmmssfff}.jpg");
                    frame.Save(fileName, ImageFormat.Jpeg);
                    Console.WriteLine($"Photo saved: {fileName}");
                }

                videoSource.SignalToStop();
                videoSource.WaitForStop();
            };

            videoSource.Start();
        }
    }
}
