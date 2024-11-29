using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace CourseWorkServer
{
    public class ServerConnector
    {
        private readonly string _clientIp;
        private readonly int _clientPort;

        public ServerConnector(string clientIp, int clientPort)
        {
            _clientIp = clientIp;
            _clientPort = clientPort;
        }

        public void RequestData(string command)
        {
            using (var client = new TcpClient(_clientIp, _clientPort))
            using (var stream = client.GetStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                writer.WriteLine(command);
                Console.WriteLine($"Відправлено команду: {command}");

                string response;
                while ((response = reader.ReadLine()) != "END_FILE")
                {
                    if (response == "START_FILE")
                    {
                        string fileName = reader.ReadLine();
                        string fileData = reader.ReadToEnd();

                        string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
                        File.WriteAllText(savePath, fileData);
                        Console.WriteLine($"Файл збережено: {savePath}");
                    }
                }
            }
        }
    }
}
