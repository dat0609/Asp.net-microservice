using Contracts.Common.Interfaces;
using Customer.API.Context;
using Customer.API.Repositories.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace Customer.API.Repositories;

public class CustomerRepository : RepositoryBaseAsync<Entities.Customer, int, CustomerContext>, ICustomerRepository
{
    public CustomerRepository(CustomerContext dbContext, IUnitOfWork<CustomerContext> unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public Task<Entities.Customer?> GetCustomerByUsername(string username) => FindByCondition(c => c.UserName == username).FirstOrDefaultAsync();

    public async Task<IEnumerable<Entities.Customer>> GetCustomersAsync()
        => await FindAll().ToListAsync();
}