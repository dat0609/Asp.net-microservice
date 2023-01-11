using Basket.API.Service.Interface;
using Shared.Configurations;

namespace Basket.API.Service;

public class BasketEmailTemplateService : EmailTemplateService, IEmailTemplateService
{
    public BasketEmailTemplateService(BackgroundJobSettings backgroundJobSettings) : base(backgroundJobSettings)
    {
    }
    
    public string GenerateReminderEmail(string username, string checkoutUrl = "basket/checkout")
    {
        var _checkoutUrl = $"{_backgroundJobSettings.CheckoutUrl}/{checkoutUrl}/{username}";
        
        var emailText = ReadEmailTemplate("reminder-checkout");
        var emailReplace = emailText.Replace("[username]", username)
            .Replace("[checkoutUrl]", _checkoutUrl);
        
        return emailReplace;
    }


}