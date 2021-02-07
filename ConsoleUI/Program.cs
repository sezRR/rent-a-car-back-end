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
            //CarTest();
            //BrandTest();
            //ColorTest();

            CarDetailsTest();
        }

        private static void CarDetailsTest()
        {
            CarManager carManager = new CarManager(new EfCarDal());

            foreach (var car in carManager.GetCarDetails())
            {
                Console.WriteLine($"{car.Description} | {car.BrandName} | {car.ColorName} | ({car.DailyPrice}$)");
            }
        }

        private static void ColorTest()
        {
            ColorManager colorManager = new ColorManager(new EfColorDal());

            //colorManager.Add(new Color { ColorId = 1, ColorName = "Blue Color" });
            //colorManager.Add(new Color { ColorId = 2, ColorName = "Black Color" });
            //colorManager.Add(new Color { ColorId = 3, ColorName = "Pink Color" });
            //colorManager.Add(new Color { ColorId = 4, ColorName = "Gray Color" });

            foreach (var color in colorManager.GetAll())
            {
                Console.WriteLine($"{color.ColorName}");
            }
        }

        private static void BrandTest()
        {
            BrandManager brandManager = new BrandManager(new EfBrandDal());

            brandManager.Add(new Brand { BrandId = 1, BrandName = "First Brand of Brands" });
            brandManager.Add(new Brand { BrandId = 2, BrandName = "Second Brand of Brands" });
            brandManager.Add(new Brand { BrandId = 3, BrandName = "Third Brand of Brands" });
            brandManager.Add(new Brand { BrandId = 4, BrandName = "Fourth Brand of Brands" });

            foreach (var brand in brandManager.GetAll())
            {
                Console.WriteLine($"{brand.BrandName}");
            }
        }

        private static void CarTest()
        {
            CarManager carManager = new CarManager(new EfCarDal());

            //carManager.Add(new Car { CarId = 1, BrandId = 1, ColorId = 2, DailyPrice = 500, Description = "very clean rentable mercedes", ModelYear = 2019 });
            //carManager.Add(new Car { CarId = 2, BrandId = 1, ColorId = 1, DailyPrice = 750, Description = "very clean rentable audi", ModelYear = 2020 });
            //carManager.Add(new Car { CarId = 2, BrandId = 1, ColorId = 1, DailyPrice = 435, Description = "very clean rentable skoda", ModelYear = 2015 });
            //carManager.Add(new Car { CarId = 3, BrandId = 2, ColorId = 2, DailyPrice = 300, Description = "very clean rentable fiat", ModelYear = 2014 });
            //carManager.Add(new Car { CarId = 4, BrandId = 2, ColorId = 1, DailyPrice = 250, Description = "very clean rentable dacia", ModelYear = 2013 });

            //carManager.Add(new Car { CarId = 4, BrandId = 2, ColorId = 1, DailyPrice = 0, Description = "e", ModelYear = 2013 });       // Error in Business (both)
            //carManager.Add(new Car { CarId = 4, BrandId = 2, ColorId = 1, DailyPrice = 1000, Description = "e", ModelYear = 2013 });    // Error in Business (description)
            //carManager.Add(new Car { CarId = 4, BrandId = 2, ColorId = 1, DailyPrice = 0, Description = "error", ModelYear = 2013 });   // Error in Business (daily price)


            foreach (var car in carManager.GetAll())
            {
                Console.WriteLine($"{car.Description} | {car.BrandId} ({car.DailyPrice}$)");
            }
        }
    }
}
