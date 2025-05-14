using System.Reflection;
using AuthService.Domain;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Interfaces;

namespace AuthService.Infrastructure.Data;

public class AuthDbContext : DbContext
{
    private readonly IDomainEventDispatcher _dispatcher;

    public AuthDbContext(DbContextOptions<AuthDbContext> options, IDomainEventDispatcher dispatcher)
        : base(options)
    {
        _dispatcher = dispatcher;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Ignore<SharedKernel.DomainEventBase>();

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