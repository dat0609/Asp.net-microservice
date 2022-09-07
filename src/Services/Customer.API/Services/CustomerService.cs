using Customer.API.Repositories.Interfaces;
using Customer.API.Services.Interfaces;

namespace Customer.API.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IResult> GetCustomerByUsernameAsync(string username)
        => Results.Ok(await _customerRepository.GetCustomerByUsername(username));

    public async Task<IResult> GetCustomersAsync() => Results.Ok(await _customerRepository.GetCustomersAsync());
    public async Task<int> CreateAsync(Entities.Customer customer)
    {
        var result = await _customerRepository.CreateAsync(customer);
        await _customerRepository.SaveChangesAsync();

        return result;
    }
}