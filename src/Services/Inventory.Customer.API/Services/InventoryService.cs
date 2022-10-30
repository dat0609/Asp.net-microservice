using AutoMapper;
using Infrastructure.Common.Repositories;
using Infrastructure.Extensions;
using Inventory.Customer.API.Entities;
using Inventory.Customer.API.Extension;
using Inventory.Customer.API.Services.Interface;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Configurations;
using Shared.DTOs;
using Shared.SeedWork;

namespace Inventory.Customer.API.Services;

public class InventoryService : MongoDbRepository<InventoryEntry>, IInventoryService
{
    private readonly IMapper _mapper;
    public InventoryService(IMongoClient client, MongoDbSettings settings, IMapper mapper) : base(client, settings)
    {
        _mapper = mapper;
    }

    public async Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync(string itemNo)
    {
        var entities = await FindAll()
            .Find(x => x.ItemNo == itemNo).ToListAsync();
        
        var result = _mapper.Map<IEnumerable<InventoryEntryDto>>(entities);
        
        return result;
    }

    public async Task<PagedList<InventoryEntryDto>> GetAllByItemNoPagingAsync(GetInventoryPagingQuery query)
    {
        var filterSearchTerm = Builders<InventoryEntry>.Filter.Empty;
        var filterItemNo = Builders<InventoryEntry>.Filter.Eq(x => x.ItemNo, query.ItemNo());

        if (!string.IsNullOrEmpty(query.SearchTerm))
            filterSearchTerm = Builders<InventoryEntry>.Filter.Eq(x => x.DocumentNo, query.SearchTerm);

        var andFilter = filterItemNo & filterSearchTerm;

        var pageList = await GetCollection.PagedListAsync(andFilter, query.PageIndex, query.PageSize);

        var items = _mapper.Map<IEnumerable<InventoryEntryDto>>(pageList);

        var result =
            new PagedList<InventoryEntryDto>(items, pageList.GetMetaData().TotalItems, query.PageIndex, query.PageSize);
        
        return result;
    }

    public async Task<InventoryEntryDto> GetByIdAsync(string id)
    {
        var filter = Builders<InventoryEntry>.Filter.Eq(x => x.Id, id);
        
        var entity = await FindAll().Find(filter).FirstOrDefaultAsync();
        
        var result = _mapper.Map<InventoryEntryDto>(entity);
        
        return result;
    }

    public async Task<InventoryEntryDto> PurchaseItemAsync(string id, PurchaseProductDto product)
    {
        var itemToAdd = new InventoryEntry(ObjectId.GenerateNewId().ToString())
        {
            ItemNo = id,
            Quantity = product.Quantity,
            DocumentType = product.DocumentType
        };
        await CreateAsync(itemToAdd);
        
        var result = _mapper.Map<InventoryEntryDto>(itemToAdd);
        
        return result;
    }
}