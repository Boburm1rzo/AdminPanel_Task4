using System.Text;

namespace Task4.Helpers;

internal static class EmailTemplateHelper
{
    public static string BuildConfirmationBody(string confirmationLink)
    {
        var htmlPath = Path.Combine(AppContext.BaseDirectory, "Helpers", "EmailConfirmation.html");
        var cssPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "css", "emailStyles.css");

        if (!File.Exists(htmlPath))
            throw new FileNotFoundException("EmailConfirmation.html not found", htmlPath);

        if (!File.Exists(cssPath))
            throw new FileNotFoundException("EmailStyles.css not found", cssPath);

        var html = File.ReadAllText(htmlPath, Encoding.UTF8);
        var css = File.ReadAllText(cssPath, Encoding.UTF8);

        return html
            .Replace("{{styles}}", css)
            .Replace("{{confirmation_link}}", confirmationLink);
    }
}
