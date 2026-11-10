using Rentar.Shared.Kernel.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Rentar.Shared.Application.Data;

public interface IEntityRepository<TEntity> where TEntity : class, IEntity, new()
{
    List<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null);
    TEntity Get(Expression<Func<TEntity, bool>> filter);

    void Add(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}
