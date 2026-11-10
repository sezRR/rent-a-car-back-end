using Microsoft.EntityFrameworkCore;
using Rentar.Shared.Application.Data;
using Rentar.Shared.Kernel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Rentar.Shared.Infrastructure.Persistence;

/// <summary>
/// Opens a short-lived context per operation, so repositories stay safe to share as singletons. Inside an
/// ambient <see cref="System.Transactions.TransactionScope"/> Npgsql hands every context the same physical
/// connection, which keeps a multi-step operation in one local transaction.
/// </summary>
public abstract class EfEntityRepositoryBase<TEntity, TContext>(IDbContextFactory<TContext> contextFactory) : IEntityRepository<TEntity>
    where TEntity : class, IEntity, new()
    where TContext : DbContext
{
    protected IDbContextFactory<TContext> ContextFactory { get; } = contextFactory;

    public void Add(TEntity entity)
    {
        using var context = ContextFactory.CreateDbContext();
        context.Entry(entity).State = EntityState.Added;
        context.SaveChanges();
    }

    public void Delete(TEntity entity)
    {
        using var context = ContextFactory.CreateDbContext();
        context.Entry(entity).State = EntityState.Deleted;
        context.SaveChanges();
    }

    public TEntity Get(Expression<Func<TEntity, bool>> filter)
    {
        using var context = ContextFactory.CreateDbContext();
        return context.Set<TEntity>().SingleOrDefault(filter);
    }

    public List<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null)
    {
        using var context = ContextFactory.CreateDbContext();
        return filter == null
            ? context.Set<TEntity>().ToList()
            : context.Set<TEntity>().Where(filter).ToList();
    }

    public void Update(TEntity entity)
    {
        using var context = ContextFactory.CreateDbContext();
        context.Entry(entity).State = EntityState.Modified;
        context.SaveChanges();
    }
}
