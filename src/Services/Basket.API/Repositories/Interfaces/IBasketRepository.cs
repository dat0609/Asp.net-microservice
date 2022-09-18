using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.API.Repositories.Interfaces;

public interface IBasketRepository
{
    Task<Cart?> GetBasketByUserName(string userName);
    Task<Cart> UpdateBasket(Cart basket, DistributedCacheEntryOptions options = null);
    Task DeleteBasketFromUserName(string userName);
}