using Contracts.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common;

public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
{
    private readonly TContext _context;

    public UnitOfWork(TContext context)
    {
        _context = context;
    }

    public void Dispose() => _context.Dispose();

    public Task<int> CommitAsync() => _context.SaveChangesAsync();
}