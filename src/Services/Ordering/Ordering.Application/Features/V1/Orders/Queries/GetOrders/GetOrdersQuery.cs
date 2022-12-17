using MediatR;
using Ordering.Application.Common.Models;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class GetOrdersQuery : IRequest<ApiResult<List<OrderDto>>>
{
    public string UserName { get; set; }

    public GetOrdersQuery(string userName)
    {
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
    }
}