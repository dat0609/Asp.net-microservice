using MediatR;
using Ordering.Application.Common.Exceptions;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;
using Serilog;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, ApiResult<bool>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger _logger;

    public DeleteOrderCommandHandler(IOrderRepository orderRepository, ILogger logger)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApiResult<bool>> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var orderEntity = await _orderRepository.GetByIdAsync(request.Id);
        if (orderEntity == null) throw new NotFoundException(nameof(Order), request.Id);
        
        orderEntity.DeletedOrder();

        _orderRepository.DeleteOrder(orderEntity);
        await _orderRepository.SaveChangesAsync();

        _logger.Information($"Order {orderEntity.Id} was successfully deleted.");

        return new ApiResult<bool>(true);
    }
}