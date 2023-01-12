using Basket.API.Service.Interface;
using Shared.Configurations;

namespace Basket.API.Service;

public class BasketEmailTemplateService : EmailTemplateService, IEmailTemplateService
{
    public BasketEmailTemplateService(BackgroundJobSettings backgroundJobSettings) : base(backgroundJobSettings)
    {
    }
    
    public string GenerateReminderEmail(string username)
    {
        var _checkoutUrl = $"{_backgroundJobSettings.CheckoutUrl}/{_backgroundJobSettings.ScheduledJobUrl}/{username}";
        
        var emailText = ReadEmailTemplate("reminder-checkout");
        var emailReplace = emailText.Replace("[username]", username)
            .Replace("[checkoutUrl]", _checkoutUrl);
        
        return emailReplace;
    }


}