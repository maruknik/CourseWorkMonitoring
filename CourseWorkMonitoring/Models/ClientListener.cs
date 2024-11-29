using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CourseWorkMonitoring.Models;

namespace CourseWorkMonitoring
{
    public class ClientListener
    {
        private readonly Screenshot _screenshot = new Screenshot();
        private readonly Webcam _webcam = new Webcam();
        private readonly string _serverIp;
        private readonly int _serverPort;

        public ClientListener(string serverIp, int serverPort)
        {
            _serverIp = serverIp;
            _serverPort = serverPort;
        }

        public void StartListening()
        {
            TcpListener listener = new TcpListener(System.Net.IPAddress.Any, _serverPort);
            listener.Start();

            while (true)
            {
                var client = listener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(HandleClient, client);
            }
        }

        private void HandleClient(object clientObj)
        {
            using (var client = (TcpClient)clientObj)
            using (var stream = client.GetStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                string request = reader.ReadLine();

                string saveDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ClientData");

                switch (request)
                {
                    case "SCREENSHOT":
                        _screenshot.TakeScreenshot(saveDir);
                        SendFile(writer, Path.Combine(saveDir, Directory.GetFiles(saveDir).Last()));
                        break;

                    case "WEBCAM":
                        _webcam.CapturePhoto(saveDir);
                        Thread.Sleep(1000); // Час для збереження фото
                        SendFile(writer, Path.Combine(saveDir, Directory.GetFiles(saveDir).Last()));
                        break;

                    default:
                        writer.WriteLine("UNKNOWN_COMMAND");
                        break;
                }
            }
        }

        private void SendFile(StreamWriter writer, string filePath)
        {
            writer.WriteLine("START_FILE");
            writer.WriteLine(Path.GetFileName(filePath));
            writer.WriteLine(File.ReadAllText(filePath));
            writer.WriteLine("END_FILE");
            Console.WriteLine($"Відправлено файл: {filePath}");
        }
    }
}
