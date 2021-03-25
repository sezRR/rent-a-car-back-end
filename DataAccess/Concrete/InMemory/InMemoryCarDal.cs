using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DataAccess.Concrete.InMemory
{
    public class InMemoryCarDal : ICarDal
    {
        List<Car> _cars;

        public InMemoryCarDal()
        {
            _cars = new List<Car>
            {
                new Car {BrandId = 1, ColorId = 1, DailyPrice = 675, ModelYear = 2018, Description = "VOLKSWAGEN UP OR SIMILAR | MINI CLASS"},
                new Car {BrandId = 1, ColorId = 1, DailyPrice = 1000, ModelYear = 2020, Description = "VOLKSWAGEN UP OR SIMILAR | MINI CLASS"},
                new Car {BrandId = 2, ColorId = 1, DailyPrice = 890, ModelYear = 2019, Description = "KIA PICANTO 2DR"},
                new Car {BrandId = 2, ColorId = 2, DailyPrice = 675, ModelYear = 2018, Description = "KIA PICANTO 2DR"},
                new Car {BrandId = 3, ColorId = 3, DailyPrice = 1200, ModelYear = 2020, Description = "FIAT 500"},
            };
        }

        public List<Car> GetAll()
        {
            return _cars;
        }

        public List<Car> GetById(int carId)
        {
            return _cars.Where(c => c.Id == carId).ToList();
        }

        public void Add(Car car)
        {
            _cars.Add(car);
        }

        public void Update(Car car)
        {
            Car carToUpdate = _cars.SingleOrDefault(c => c.Id == car.Id);

            carToUpdate.BrandId = car.BrandId;
            carToUpdate.ColorId = car.ColorId;
            carToUpdate.DailyPrice = car.DailyPrice;
            carToUpdate.ModelYear = car.ModelYear;
            carToUpdate.Description = car.Description;
        }

        public void Delete(Car car)
        {
            Car carToDelete = _cars.SingleOrDefault(c => c.Id == car.Id);

            _cars.Remove(carToDelete);
        }

        public List<Car> GetAll(Expression<Func<Car, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public Car Get(Expression<Func<Car, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public List<CarDetailDto> GetCarDetails(Expression<Func<Car, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public CarDetailDto GetCarDetailsById(Expression<Func<Car, bool>> filter)
        {
            throw new NotImplementedException();
        }
    }
}
