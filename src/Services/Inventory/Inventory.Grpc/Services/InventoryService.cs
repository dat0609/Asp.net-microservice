using Grpc.Core;
using Inventory.Grpc.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace Inventory.Grpc.Services;
using Protos;
public class InventoryService : StockProtoService.StockProtoServiceBase
{
    private readonly IInventoryRepository _repository;
    private readonly ILogger _logger;

    public InventoryService(IInventoryRepository repository, ILogger logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<StockModel> GetStock(GetStockRequest request, ServerCallContext context)
    {
        _logger.Information($"BEGIN Get Stock of ItemNo: {request.ItemNo}");
        var stockQuantity = await _repository.GetStockQuantity(request.ItemNo);
        var result = new StockModel()
        {
            Quantity = stockQuantity
        };
        _logger.Information($"END Get Stock of ItemNo: {request.ItemNo} - Quantity: {result.Quantity}");

        return result;
    }
}