using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Contracts.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.API.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly IDistributedCache _redisCacheService;
    private readonly ISerializerService _serializerService;

    public BasketRepository(IDistributedCache redisCacheService, ISerializerService serializerService)
    {
        _redisCacheService = redisCacheService;
        _serializerService = serializerService;
    }

    public async Task<Cart?> GetBasketByUserName(string userName)
    {
        var basket = await _redisCacheService.GetStringAsync(userName);
        return string.IsNullOrEmpty(basket) ? null :
            _serializerService.Deserialize<Cart>(basket);
    }

    public async Task<Cart> UpdateBasket(Cart basket, DistributedCacheEntryOptions options)
    {
        if (options != null)
            await _redisCacheService.SetStringAsync(basket.UserName,
                _serializerService.Serialize(basket), options);
        else
            await _redisCacheService.SetStringAsync(basket.UserName,
                _serializerService.Serialize(basket));

        return await GetBasketByUserName(basket.UserName);
    }

    public Task DeleteBasketFromUserName(string userName)
        => _redisCacheService.RemoveAsync(userName);
}