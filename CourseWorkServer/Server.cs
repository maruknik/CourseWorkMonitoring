using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CourseWorkServer
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
            cbCommands.Items.Add("SCREENSHOT");
            cbCommands.Items.Add("WEBCAM");

            cbCommands.SelectedIndex = 0;
        }

        private void Server_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnSendCommand_Click(object sender, EventArgs e)
        {
            string command = cbCommands.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(command))
            {
                AppendLog("Оберіть команду перед відправкою.");
                return;
            }

            string clientIp = txtClientIp.Text;
            int clientPort = int.Parse(txtClientPort.Text);

            try
            {
                using (var client = new TcpClient(clientIp, clientPort))
                using (var stream = client.GetStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                    writer.WriteLine(command);
                    AppendLog($"Відправлено команду: {command}");

                    string response;
                    MemoryStream imageStream = new MemoryStream();

                    while ((response = reader.ReadLine()) != "END_FILE")
                    {
                        if (response == "START_FILE")
                        {
                            string fileName = reader.ReadLine();
                            string fileData = reader.ReadLine(); 

                            byte[] fileBytes = Convert.FromBase64String(fileData);
                            imageStream = new MemoryStream(fileBytes);

                            AppendLog($"Файл отримано: {fileName}");
                        }
                    }

                    if (imageStream.Length > 0)
                    {
                        pictureBoxDisplay.Image = Image.FromStream(imageStream);
                        AppendLog("Зображення відображено.");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Помилка: {ex.Message}");
            }
        }

        private void AppendLog(string message)
        {
            txtLog.AppendText(message + Environment.NewLine);
        }
    }
}
