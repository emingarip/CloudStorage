using System.Reflection;
using FileMetadataService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Interfaces;

namespace FileMetadataService.Infrastructure.Data;

public class FileMetadataDbContext : DbContext
{
    private readonly IDomainEventDispatcher _dispatcher;

    public FileMetadataDbContext(DbContextOptions<FileMetadataDbContext> options, IDomainEventDispatcher dispatcher)
        : base(options)
    {
        _dispatcher = dispatcher;
    }

    public DbSet<FileEntity> Files { get; set; }
    public DbSet<Domain.Entities.FileShare> FileShares { get; set; }
    public DbSet<FileVersion> FileVersions { get; set; }
    public DbSet<FileActivity> FileActivities { get; set; }

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