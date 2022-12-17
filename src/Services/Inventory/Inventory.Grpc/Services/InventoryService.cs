using Grpc.Core;
using Inventory.Grpc.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace Inventory.Grpc.Services;
using Inventory.Grpc.Protos;

public class InventoryService : StockProtoService.StockProtoServiceBase
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILogger _logger;

    public InventoryService(IInventoryRepository inventoryRepository, ILogger logger)
    {
        _inventoryRepository = inventoryRepository ?? throw new ArgumentNullException(nameof(inventoryRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public override async Task<StockModel> GetStock(GetStockRequest request, ServerCallContext context)
    {
        _logger.Information($"GetStock called: {request.ItemNo}");
        var stock = await _inventoryRepository.GetStockQuantity(request.ItemNo);
        
        var result = new StockModel
        {
            Quantity = stock
        };
        
        return  result;
    }
}