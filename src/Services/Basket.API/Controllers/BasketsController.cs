using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories.Interfaces;
using Basket.API.Service.Interface;
using EventBus.Messages.IntegrationEvents.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Shared.DTOs.Basket;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketsController : ControllerBase
{
    private readonly IBasketRepository _basketRepository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly StockItemGrpcService _stockItemGrpcService;
    private readonly IEmailTemplateService _emailTemplateService;

    public BasketsController(IBasketRepository basketRepository, IMapper mapper, IPublishEndpoint publishEndpoint, StockItemGrpcService stockItemGrpcService, IEmailTemplateService emailTemplateService)
    {
        _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _stockItemGrpcService = stockItemGrpcService ?? throw new ArgumentNullException(nameof(stockItemGrpcService));
        _emailTemplateService = emailTemplateService;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet("{username}", Name = "GetBasket")]
    [ProducesResponseType(typeof(CartDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CartDto>> GetBasket([Required] string username)
    {
        var result = await _basketRepository.GetBasketByUserName(username);

        return Ok(result ?? new Cart(username));
    }
    
    [HttpPost(Name = "UpdateBasket")]
    [ProducesResponseType(typeof(CartDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CartDto>> UpdateBasket([FromBody] CartDto model)
    {
        // Communicate with Inventory.Product.Grpc and check quantity available of products
        foreach (var item in model.Items)
        {
            var stock = await _stockItemGrpcService.GetStock(item.ItemNo);
            item.SetAvailableQuantity(stock.Quantity);
        }
        
        var options = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(DateTime.UtcNow.AddHours(10));
        //     .SetSlidingExpiration(TimeSpan.FromMinutes(10));
        
        var cart = _mapper.Map<Cart>(model);
        
        var updateCart = await _basketRepository.UpdateBasket(cart, options);

        var result = _mapper.Map<CartDto>(updateCart);
        
        return Ok(result);
    }
    
    [HttpDelete("{username}", Name = "DeleteBasket")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> DeleteBasket([Required] string username)
    {
        var result = await _basketRepository.DeleteBasketFromUserName(username);
        return Ok(result);
    }

    [Route("[action]")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
    {
        var basket = await _basketRepository.GetBasketByUserName(basketCheckout.UserName);
        if (basket == null) return NotFound();
        
        //publish checkout event to EventBus Message
        var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
        eventMessage.TotalPrice = basket.TotalPrice;
        await _publishEndpoint.Publish(eventMessage);
        //remove the basket
        await _basketRepository.DeleteBasketFromUserName(basket.Username);
        
        return Accepted();
    }
    
    [HttpPost("email")]
    public ContentResult SendEmail()
    {
        var result = _emailTemplateService.GenerateReminderEmail("datlqse140263@fpt.edu.vn");

        var rs = new ContentResult
        {
            Content = result,
            ContentType = "text/html",
        };
            
        return rs;
    }
}