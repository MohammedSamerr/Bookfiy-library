﻿namespace Bookfiy_WepApp.Services
{
    public interface IEmailBodyBuilder
    {
        string GetEmailBody(string imageUrl, string header, string body, string url, string link);
    }
}