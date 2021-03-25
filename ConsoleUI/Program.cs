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

            //CarDetailsTest();

            //CustomerTest();
            //UserTest();

            //RentalTest();

            RentalDetailsTest();
        }

        private static void RentalDetailsTest()
        {
            RentalManager rentalManager = new RentalManager(new EfRentalDal());

            var result = rentalManager.GetRentalDetails();

            if (result.Success)
            {
                foreach (var rental in result.Data)
                {
                    //Console.WriteLine($"{rental.Id} | {rental.UserName} | {rental.CustomerName} | {rental.Description} | {rental.RentDate} | {rental.ReturnDate} (Rental Id - UserName - CustomerName - Description - RentDate - ReturnDate)");
                }
            }
            else
            {
                Console.WriteLine(result.Message);
            }
        }

        private static void RentalTest()
        {
            RentalManager rentalManager = new RentalManager(new EfRentalDal());

            //Rental rental1 = new Rental
            //{
            //    CarId = 1,
            //    CustomerId = 1,
            //    RentDate = new DateTime(2021, 02, 13),
            //    ReturnDate = new DateTime(2021, 02, 20)
            //};

            //Rental rental2 = new Rental
            //{
            //    CarId = 3,
            //    CustomerId = 1,
            //    RentDate = new DateTime(2021, 02, 21),
            //    ReturnDate = new DateTime(2021, 02, 26)
            //};

            //Rental rental3 = new Rental
            //{
            //    CarId = 2,
            //    CustomerId = 2,
            //    RentDate = new DateTime(2021, 02, 16),
            //    ReturnDate = new DateTime(2021, 02, 25)
            //};

            //rentalManager.Add(rental1);
            //rentalManager.Add(rental2);
            //rentalManager.Add(rental3);

            var result = rentalManager.GetAll();

            if (result.Success)
            {
                foreach (var rental in result.Data)
                {
                    Console.WriteLine($"{rental.Id} | {rental.CarId} | {rental.CustomerId} | {rental.RentDate} | {rental.ReturnDate} (RentalId - CarId - CustomerId - RentDate - ReturnDate)");
                }
            }
            else
            {
                Console.WriteLine(result.Message);
            }
        }

        private static void UserTest()
        {
            UserManager userManager = new UserManager(new EfUserDal());

            //User user1 = new User
            //{
            //    FirstName = "Faruk",
            //    LastName = "Demir",
            //    Email = "farukdemir@email.com",
            //    Password = "farukdemir123"
            //};

            //User user2 = new User
            //{
            //    FirstName = "Necmi",
            //    LastName = "Uzungil",
            //    Email = "necmiuzungil@email.com",
            //    Password = "necmiuzungil123"
            //};

            //User user3 = new User
            //{
            //    FirstName = "Haluk",
            //    LastName = "Kaya",
            //    Email = "halukkaya@email.com",
            //    Password = "halukkaya123"
            //};

            //User user4 = new User
            //{
            //    FirstName = "Ertan",
            //    LastName = "Tuğ",
            //    Email = "ertantug@email.com",
            //    Password = "ertantug123"
            //};

            //User user5 = new User
            //{
            //    FirstName = "Veli",
            //    LastName = "Denk",
            //    Email = "velidenk@email.com",
            //    Password = "velidenk123"
            //};

            //userManager.Add(user1);
            //userManager.Add(user2);
            //userManager.Add(user3);
            //userManager.Add(user4);
            //userManager.Add(user5);

            var result = userManager.GetAll();

            if (result.Success)
            {
                foreach (var user in result.Data)
                {
                    Console.WriteLine($"{user.Id} | {user.FirstName} | {user.LastName} | {user.Email} (Id - FirstName - LastName - Email)");
                }
            }
            else
            {
                Console.WriteLine(result.Message);
            }
        }

        private static void CustomerTest()
        {
            CustomerManager customerManager = new CustomerManager(new EfCustomerDal());

            //Customer customer1 = new Customer
            //{
            //    UserId = 1,
            //    CompanyName = "Company1"
            //};

            //Customer customer2 = new Customer
            //{
            //    UserId = 1,
            //    CompanyName = "Company1.2"
            //};

            //Customer customer3 = new Customer
            //{
            //    UserId = 2,
            //    CompanyName = "Company2"
            //};

            //Customer customer4 = new Customer
            //{
            //    UserId = 3,
            //    CompanyName = "Company3"
            //};

            //Customer customer5 = new Customer
            //{
            //    UserId = 4,
            //    CompanyName = "Company4"
            //};

            //customerManager.Add(customer1);
            //customerManager.Add(customer2);
            //customerManager.Add(customer3);
            //customerManager.Add(customer4);
            //customerManager.Add(customer5);

            var result = customerManager.GetAll();

            if (result.Success)
            {
                foreach (var car in result.Data)
                {
                    Console.WriteLine($"{car.Id} | {car.UserId} | {car.CompanyName} (Id - UserId - CompanyName)");
                }
            }
            else
            {
                Console.WriteLine(result.Message);
            }
        }

        private static void CarDetailsTest()
        {
            CarManager carManager = new CarManager(new EfCarDal());

            var result = carManager.GetCarDetails();

            if (result.Success)
            {
                foreach (var car in result.Data)
                {
                    Console.WriteLine($"{car.Description} | {car.BrandName} | {car.ColorName} | ({car.DailyPrice}$)");
                }
            }
            else
            {
                Console.WriteLine(result.Message);
            }
        }

        private static void ColorTest()
        {
            ColorManager colorManager = new ColorManager(new EfColorDal());

            //colorManager.Add(new Color { ColorId = 1, ColorName = "Blue Color" });
            //colorManager.Add(new Color { ColorId = 2, ColorName = "Black Color" });
            //colorManager.Add(new Color { ColorId = 3, ColorName = "Pink Color" });
            //colorManager.Add(new Color { ColorId = 4, ColorName = "Gray Color" });

            var result = colorManager.GetAll();

            if (result.Success)
            {
                foreach (var color in result.Data)
                {
                    Console.WriteLine(color.Id + "/" + color.ColorName);
                }
            }
            else
            {
                Console.WriteLine(result.Message);
            }
        }

        private static void BrandTest()
        {
            BrandManager brandManager = new BrandManager(new EfBrandDal());

            //brandManager.Add(new Brand { BrandId = 1, BrandName = "First Brand of Brands" });
            //brandManager.Add(new Brand { BrandId = 2, BrandName = "Second Brand of Brands" });
            //brandManager.Add(new Brand { BrandId = 3, BrandName = "Third Brand of Brands" });
            //brandManager.Add(new Brand { BrandId = 4, BrandName = "Fourth Brand of Brands" });

            var result = brandManager.GetAll();

            if (result.Success)
            {
                foreach (var brand in result.Data)
                {
                    Console.WriteLine(brand.Id + "/" + brand.BrandName);
                }
            }
            else
            {
                Console.WriteLine(result.Message);
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

            var result = carManager.GetAll();

            if (result.Success)
            {
                foreach (var car in result.Data)
                {
                    Console.WriteLine($"{car.Description} | {car.BrandId} | ({car.DailyPrice}$)");
                }
            }
            else
            {
                Console.WriteLine(result.Message);
            }
        }
    }
}
