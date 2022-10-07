using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Repositories;

public class OrderRepository : RepositoryBase<Order, long, OrderContext>, IOrderRepository
{
    public OrderRepository(OrderContext dbContext, IUnitOfWork<OrderContext> unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName) =>
        await FindByCondition(x => x.UserName.Equals(userName)).ToListAsync();
}