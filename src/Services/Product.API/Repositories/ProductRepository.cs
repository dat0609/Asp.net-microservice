using Contracts.Domains.Interfaces;
using Infrastructure.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.API.Entities;
using Product.API.Persistence;
using Product.API.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace Product.API.Repositories;

public class ProductRepository : RepositoryBase<CatalogProduct, long, ProductContext>, IProductRepository
{
    public ProductRepository(ProductContext dbContext, IUnitOfWork<ProductContext> unitOfWork) : base(dbContext, unitOfWork)
    {
    }
   
    public async Task<IEnumerable<CatalogProduct>> GetProductsAsync() => await FindAll().ToListAsync();

    public Task<CatalogProduct> GetProductAsync(long id) => GetByIdAsync(id);

    public Task<CatalogProduct> GetProductByNoAsync(string productNo) =>
        FindByCondition(x => x.No.Equals(productNo)).SingleOrDefaultAsync();

    public Task CreateProductAsync(CatalogProduct product) => CreateAsync(product);

    public Task UpdateProductAsync(CatalogProduct product) => UpdateAsync(product);

    public async Task DeleteProductAsync(long id)
    {
        var product = await GetProductAsync(id);
        if (product != null) await DeleteAsync(product);
    }
}