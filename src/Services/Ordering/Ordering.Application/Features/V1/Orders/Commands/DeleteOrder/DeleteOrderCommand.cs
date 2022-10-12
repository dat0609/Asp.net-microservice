using MediatR;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class DeleteOrderCommand : IRequest<ApiResult<bool>>
{
    public long Id { get; private set; }

    public DeleteOrderCommand(long id)
    {
        Id = id;
    }
}