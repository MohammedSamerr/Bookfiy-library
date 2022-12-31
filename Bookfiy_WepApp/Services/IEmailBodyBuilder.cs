namespace Bookfiy_WepApp.Services
{
    public interface IEmailBodyBuilder
    {
        string GetEmailBody(string template, Dictionary<string, string> placeholders);
    }
}
