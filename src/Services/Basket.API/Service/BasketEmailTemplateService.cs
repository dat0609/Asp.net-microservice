using Basket.API.Service.Interface;

namespace Basket.API.Service;

public class BasketEmailTemplateService : EmailTemplateService, IEmailTemplateService
{
    public string GenerateReminderEmail(string email, string username)
    {
        var checkoutUrl = "http://localhost:5001/basket/checkout";
        var emailText = ReadEmailTemplate("reminder-checkout");
        var emailReplace = emailText.Replace("[username]", username)
            .Replace("[checkoutUrl]", checkoutUrl);
        
        return emailReplace;
    }
}