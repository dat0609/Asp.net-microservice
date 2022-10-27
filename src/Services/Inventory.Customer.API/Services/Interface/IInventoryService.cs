using Inventory.Customer.API.Entities;
using Inventory.Customer.API.Repositories.Abstraction;
using Shared.DTOs;
using Shared.SeedWork;

namespace Inventory.Customer.API.Services.Interface;

public interface IInventoryService : IMongoDbRepositoryBase<InventoryEntry>
{
    Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync(string itemNo);
    Task<PagedList<InventoryEntryDto>> GetAllByItemNoPagingAsync(GetInventoryPagingQuery query);
    Task<InventoryEntryDto> GetByIdAsync(string id);
    Task<InventoryEntryDto> PurchaseItemAsync(string id, PurchaseProductDto product);
}