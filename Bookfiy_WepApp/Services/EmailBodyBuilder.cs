using System.Text.Encodings.Web;

namespace Bookfiy_WepApp.Services
{
    public class EmailBodyBuilder :IEmailBodyBuilder
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EmailBodyBuilder(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string GetEmailBody(string imageUrl, string header, string body, string url, string link)
        {
            var filePath = $"{_webHostEnvironment.WebRootPath}/templates/email.html";
            StreamReader str = new(filePath);
            var temp = str.ReadToEnd();
            str.Close();
            return temp.
                Replace("[imageUrl]", imageUrl).
                Replace("[header]", header).
                Replace("[body]", body).
                Replace("[url]", url).
                Replace("[linkTitle]", link);
        }
    }
}
