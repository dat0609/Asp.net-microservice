using Microsoft.EntityFrameworkCore;

namespace Contracts.Domains.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task<int> CommitAsync();
}

public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
}