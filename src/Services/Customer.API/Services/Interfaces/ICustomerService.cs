namespace Customer.API.Services.Interfaces;

public interface ICustomerService
{
    Task<IResult> GetCustomerByUsernameAsync(string username);
    Task<IResult> GetCustomersAsync();
    Task<int> CreateAsync(Entities.Customer customer);
}