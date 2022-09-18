using Contracts.Common.Interfaces;
using Customer.API.Context;

namespace Customer.API.Repositories.Interfaces;

public interface ICustomerRepository : IRepositoryQueryBase<Entities.Customer, int, CustomerContext>
{
    Task<Entities.Customer?> GetCustomerByUsername(string username);
    Task<IEnumerable<Entities.Customer>> GetCustomersAsync();
}