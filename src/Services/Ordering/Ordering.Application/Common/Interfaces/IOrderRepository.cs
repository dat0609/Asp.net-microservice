using Contracts.Common.Interfaces;
using Ordering.Domain.Entities;

namespace Ordering.Application.Common.Interfaces;

public interface IOrderRepository : IRepositoryBaseAsync<Order, long>
{
    Task<IEnumerable<Order>> GetOrdersByUserName(string userName);
}