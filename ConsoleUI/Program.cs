using Business.Concrete;
using DataAccess.Concrete.EntityFramework;
using DataAccess.Concrete.InMemory;
using Entities.Concrete;
using System;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            CarManager carManager = new CarManager(new EfCarDal());

            carManager.Add(new Car {CarId = 1, BrandId = 1, ColorId = 2, DailyPrice = 500, Description = "very clean rentable mercedes", ModelYear = 2019 });
            carManager.Add(new Car {CarId = 2, BrandId = 1, ColorId = 1, DailyPrice = 750, Description = "very clean rentable audi", ModelYear = 2020 });
            carManager.Add(new Car {CarId = 2, BrandId = 1, ColorId = 1, DailyPrice = 435, Description = "very clean rentable skoda", ModelYear = 2015 });
            carManager.Add(new Car {CarId = 3, BrandId = 2, ColorId = 2, DailyPrice = 300, Description = "very clean rentable fiat", ModelYear = 2014 });
            carManager.Add(new Car {CarId = 4, BrandId = 2, ColorId = 1, DailyPrice = 250, Description = "very clean rentable dacia", ModelYear = 2013 });

            carManager.Add(new Car { CarId = 4, BrandId = 2, ColorId = 1, DailyPrice = 0, Description = "e", ModelYear = 2013 });       // Error in Business (both)
            carManager.Add(new Car { CarId = 4, BrandId = 2, ColorId = 1, DailyPrice = 1000, Description = "e", ModelYear = 2013 });    // Error in Business (description)
            carManager.Add(new Car { CarId = 4, BrandId = 2, ColorId = 1, DailyPrice = 0, Description = "error", ModelYear = 2013 });   // Error in Business (daily price)


            foreach (var car in carManager.GetAll())
            {
                Console.WriteLine($"{car.Description} | {car.ModelYear} ({car.DailyPrice}$)");
            }
        }
    }
}
