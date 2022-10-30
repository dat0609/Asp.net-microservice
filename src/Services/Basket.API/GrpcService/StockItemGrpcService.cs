using Inventory.Grpc.Protos;

namespace Basket.API.GrpcService;

public class StockItemGrpcService
{
    private readonly StockProtoService.StockProtoServiceClient _stockProtoServiceClient;

    public StockItemGrpcService(StockProtoService.StockProtoServiceClient stockProtoServiceClient)
    {
        _stockProtoServiceClient = stockProtoServiceClient;
    }

    public async Task<StockModel> GetStock(string itemNo)
    {
        try
        {
            var stockRequest = new GetStockRequest {ItemNo = itemNo};
            
            return _stockProtoServiceClient.GetStock(stockRequest);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}