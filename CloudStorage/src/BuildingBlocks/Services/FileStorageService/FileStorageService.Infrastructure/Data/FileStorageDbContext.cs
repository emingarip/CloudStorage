using System.Reflection;
using FileStorageService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Interfaces;

namespace FileStorageService.Infrastructure.Data;

public class FileStorageDbContext : DbContext
{
    private readonly IDomainEventDispatcher _dispatcher;

    public FileStorageDbContext(DbContextOptions<FileStorageDbContext> options, IDomainEventDispatcher dispatcher)
        : base(options)
    {
        _dispatcher = dispatcher;
    }

    public DbSet<StoredFile> StoredFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        // Dispatch domain events before saving changes
        var entities = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var entity in entities)
        {
            await _dispatcher.DispatchEventsAsync(entity);
            entity.ClearDomainEvents();
        }

        return result;
    }
}