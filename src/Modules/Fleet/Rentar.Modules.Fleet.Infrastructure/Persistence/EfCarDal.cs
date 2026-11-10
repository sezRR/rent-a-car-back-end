using Microsoft.EntityFrameworkCore;
using Rentar.Modules.Fleet.Application.Cars;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Rentar.Modules.Fleet.Infrastructure.Persistence;

public class EfCarDal(IDbContextFactory<FleetDbContext> contextFactory) : EfEntityRepositoryBase<Car, FleetDbContext>(contextFactory), ICarDal
{
    public List<CarDetailDto> GetCarDetails(Expression<Func<Car, bool>> filter = null)
    {
        using var context = ContextFactory.CreateDbContext();
        return QueryDetails(context, filter).ToList();
    }

    public CarDetailDto GetCarDetailsById(Expression<Func<Car, bool>> filter)
    {
        using var context = ContextFactory.CreateDbContext();
        return QueryDetails(context, filter).FirstOrDefault();
    }

    private static IQueryable<CarDetailDto> QueryDetails(FleetDbContext context, Expression<Func<Car, bool>> filter)
    {
        return from c in filter is null ? context.Cars : context.Cars.Where(filter)
               join b in context.Brands
               on c.BrandId equals b.Id
               join co in context.Colors
               on c.ColorId equals co.Id
               select new CarDetailDto
               {
                   Id = c.Id,
                   CarId = c.Id,
                   BrandId = c.BrandId,
                   BrandName = b.BrandName,
                   ColorId = c.ColorId,
                   Description = c.Description,
                   ModelYear = c.ModelYear,
                   ColorName = co.ColorName,
                   DailyPrice = c.DailyPrice,
                   MinimumFindeksRating = c.MinimumFindeksRating,
                   ImagePath = (from a in context.CarImages where a.CarId == c.Id select a.ImagePath).FirstOrDefault()
               };
    }
}
