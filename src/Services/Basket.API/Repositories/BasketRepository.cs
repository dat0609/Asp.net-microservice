using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Basket.API.Service;
using Basket.API.Service.Interface;
using Contracts.Common.Interfaces;
using Infrastructure.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Shared.DTOs.ScheduleJob;
using ILogger = Serilog.ILogger;

namespace Basket.API.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly IDistributedCache _redisCacheService;
    private readonly ISerializeService _serializeService;
    private readonly ILogger _logger;
    private readonly BackgroundJobHttpService _backgroundJobHttpService;
    private readonly IEmailTemplateService _emailTemplateService;

    public BasketRepository(IDistributedCache redisCacheService, ISerializeService serializeService, ILogger logger, BackgroundJobHttpService backgroundJobHttpService, IEmailTemplateService emailTemplateService)
    {
        _redisCacheService = redisCacheService;
        _serializeService = serializeService;
        _logger = logger;
        _backgroundJobHttpService = backgroundJobHttpService;
        _emailTemplateService = emailTemplateService;
    }
    
    public async Task<Cart?> GetBasketByUserName(string username)
    {
        _logger.Information($"BEGIN: GetBasketByUserName {username}");
        var basket = await _redisCacheService.GetStringAsync(username);
        _logger.Information($"END: GetBasketByUserName {username}");
        
        return string.IsNullOrEmpty(basket) ? null : _serializeService.Deserialize<Cart>(basket);
    }

    public async Task<Cart> UpdateBasket(Cart cart, DistributedCacheEntryOptions options = null)
    {
        await DeleteReminderCheckoutOrder(cart.Username);
        
        _logger.Information($"BEGIN: UpdateBasket for {cart.Username}");
        
        if (options != null)
            await _redisCacheService.SetStringAsync(cart.Username,
                _serializeService.Serialize(cart), options);
        else
            await _redisCacheService.SetStringAsync(cart.Username,
                _serializeService.Serialize(cart));
        
        _logger.Information($"END: UpdateBasket for {cart.Username}");
        
        try
        {
            await TriggerSendEmail(cart);
        }
        catch (Exception e)
        {
            _logger.Error($"Error in UpdateBasket: {e.Message}");
        }
        
        return await GetBasketByUserName(cart.Username);
    }

    private async Task TriggerSendEmail(Cart cart)
    {
        var email = _emailTemplateService.GenerateReminderEmail(cart.Username);
        
        var model = new ReminderCheckoutOrderDto(cart.EmailAddress, "Reminder Checkout Order", email, 
            DateTimeOffset.Now.AddSeconds(20));

        var uri = $"{_backgroundJobHttpService.ScheduledJobUrl}/send-email";

        var response = await _backgroundJobHttpService.Client.PostAsJson(uri, model);

        if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
        {
            var jobId = await response.ReadContentAs<string>();
            if (!string.IsNullOrEmpty(jobId))
            {
                cart.JobId = jobId;
                await _redisCacheService.SetStringAsync(cart.Username, _serializeService.Serialize(cart));
            }
        }
    }
    
    public async Task<bool> DeleteBasketFromUserName(string username)
    {
        await DeleteReminderCheckoutOrder(username);
        try
        {
            _logger.Information($"BEGIN: DeleteBasketFromUserName {username}");
            await _redisCacheService.RemoveAsync(username);
            _logger.Information($"END: DeleteBasketFromUserName {username}");

            return true;
        }
        catch (Exception e)
        {
            _logger.Error("Error DeleteBasketFromUserName: " + e.Message);
            throw;
        }
    }
    
    private async Task DeleteReminderCheckoutOrder(string username)
    {
        var cart = await GetBasketByUserName(username);
        if (cart == null || string.IsNullOrEmpty(cart.JobId)) return;
        
        var jobId = cart.JobId;
        var uri = $"{_backgroundJobHttpService.ScheduledJobUrl}/delete/jobId/{jobId}";
        
        _backgroundJobHttpService.Client.DeleteAsync(uri);
        
        _logger.Information($"DeleteReminderCheckoutOrder:Deleted JobId: {jobId}");
    }
}