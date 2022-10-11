using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using EventBus.Messages.IntegrationEvents.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _basketRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMapper _mapper;

    public BasketController(IBasketRepository basketRepository, IPublishEndpoint publishEndpoint, IMapper mapper)
    {
        _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
        _publishEndpoint = publishEndpoint;
        _mapper = mapper;
    }

    [HttpGet("{userName}", Name = "GetBasket")]
    [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Cart>> GetBasket([Required]string userName)
    {
        var result = await _basketRepository.GetBasketByUserName(userName);
        return Ok(result ?? new Cart(userName));
    }

    [HttpPost(Name = "UpdateBasket")]
    [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Cart>> UpdateBasket([FromBody] Cart basket)
    {
        var options = new DistributedCacheEntryOptions()
            //set the absolute expiration time.
            .SetAbsoluteExpiration(DateTime.UtcNow.AddMinutes(10))
            //a cached object will be expired if it not being requested for a defined amount of time period.
            //Sliding Expiration should always be set lower than the absolute expiration.
            .SetSlidingExpiration(TimeSpan.FromMinutes(2));

        var result = await _basketRepository.UpdateBasket(basket, options);
        return Ok(result);
    }

    [HttpDelete("{userName}", Name = "DeleteBasket")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.Accepted)]
    public async Task<IActionResult> DeleteBasket([Required]string userName)
    {
        await _basketRepository.DeleteBasketFromUserName(userName);
        return Accepted();
    }

    [Route("[action]/{username}")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Checkout([Required] string username, [FromBody] BasketCheckout basketCheckout)
    {
        var basket = await _basketRepository.GetBasketByUserName(username);
        if (basket == null || !basket.Items.Any()) return NotFound();

        var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);

        eventMessage.TotalPrice = basket.TotalPrice;
        //publish checkout event to EventBus Message
        await _publishEndpoint.Publish(eventMessage);

        //remove the basket
        await _basketRepository.DeleteBasketFromUserName(basket.UserName);

        return Accepted();
    }
}