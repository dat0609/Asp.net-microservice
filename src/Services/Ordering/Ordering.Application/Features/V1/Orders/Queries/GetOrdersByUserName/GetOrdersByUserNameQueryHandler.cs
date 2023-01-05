using AutoMapper;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Serilog;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class GetOrdersByUserNameQueryHandler : IRequestHandler<GetOrdersByUserNameQuery, ApiResult<List<OrderDto>>>
{
    private readonly IMapper _mapper;
    private readonly IOrderRepository _repository;
    private readonly ILogger _logger;
    
    public GetOrdersByUserNameQueryHandler(IMapper mapper, IOrderRepository repository, ILogger logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(repository));
    }

    private const string MethodName = "GetOrdersQueryHandler";
    
    public async Task<ApiResult<List<OrderDto>>> Handle(GetOrdersByUserNameQuery request, CancellationToken cancellationToken)
    {
        _logger.Information($"BEGIN: {MethodName} - Username: {request.UserName}");
        
        var orderEntities = await _repository.GetOrdersByUserNameAsync(request.UserName);
        var orderList = _mapper.Map<List<OrderDto>>(orderEntities);
        
        _logger.Information($"END: {MethodName} - Username: {request.UserName}");

        return new ApiSuccessResult<List<OrderDto>>(orderList);
    }
}