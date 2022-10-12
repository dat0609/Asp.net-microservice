using AutoMapper;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Serilog;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, ApiResult<OrderDto>>
{
    private readonly IMapper _mapper;
    private readonly IOrderRepository _repository;
    private readonly ILogger _logger;
    
    public GetOrderByIdQueryHandler(IMapper mapper, IOrderRepository repository, ILogger logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(repository));
    }
    
    private const string MethodName = "GetOrderByIdQueryHandler";
    
    public async Task<ApiResult<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.Information($"BEGIN: {MethodName} - Id: {request.Id}");

        var order = await _repository.GetByIdAsync(request.Id);
        var orderDto = _mapper.Map<OrderDto>(order);
        
        _logger.Information($"END: {MethodName} - Id: {request.Id}");

        return new ApiSuccessResult<OrderDto>(orderDto);
    }
}