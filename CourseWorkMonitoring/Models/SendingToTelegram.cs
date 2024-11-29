using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CourseWorkMonitoring.Models
{
    public class SendingToTelegram
    {
        private readonly string _botToken = "7919226501:AAGm_6UI_XgjSecSt8xtq1KpR01Ml1ayjxI";
        private readonly string _chatId = "767160871";

        public async Task SendFileAsyncTelegram(string filePath)
        {
            string url = $"https://api.telegram.org/bot{_botToken}/sendDocument";

            using (var client = new HttpClient())
            using (var form = new MultipartFormDataContent())
            {
                form.Add(new StringContent(_chatId), "chat_id");

                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var fileContent = new StreamContent(fileStream);
                form.Add(fileContent, "document", Path.GetFileName(filePath));

                var response = await client.PostAsync(url, form);
            }
        }
    }
}
