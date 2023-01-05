using Contracts.Common.Interfaces;
using Contracts.Domains.Interfaces;
using Ordering.Application.Features.V1.Orders;
using Ordering.Domain.Entities;

namespace Ordering.Application.Common.Interfaces;

public interface IOrderRepository : IRepositoryBase<Order, long>
{
    Task<IEnumerable<Order>> GetOrdersByUserNameAsync(string userName);
    void CreateOrder(Order order);
    Task<Order> UpdateOrderAsync(Order order);
    void DeleteOrder(Order order);
}