using AutoMapper;
using Inventory.Customer.API.Entities;
using Shared.DTOs;

namespace Inventory.Customer.API;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<InventoryEntry, InventoryEntryDto>().ReverseMap();
        CreateMap<InventoryEntry, PurchaseProductDto>().ReverseMap();
    }
}