using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    public class CarManager : ICarService
    {
        ICarDal _carDal;

        public CarManager(ICarDal carDal)
        {
            _carDal = carDal;
        }

        public void Add(Car car)
        {
            if (!(car.Description.Length > 2) && !(car.DailyPrice > 0))
            {
                Console.WriteLine("You need to write a explanatory description about your can be rentable car!");
                Console.WriteLine("You need to write a realistic price for your can be rentable car! (Car Daily Price must be Greater than 0$)");
            }
            else if (!(car.Description.Length > 2) || !(car.DailyPrice > 0))
            {
                if (!(car.Description.Length > 2))
                {
                    Console.WriteLine("You need to write a explanatory description about your can be rentable car!");
                }
                else
                {
                    Console.WriteLine("You need to write a realistic price for your can be rentable car! (Car Daily Price must be Greater than 0$)");
                }
            }
            else
            {
                _carDal.Add(car);
                Console.WriteLine($"Added A New Car! ({car.Description} | {car.ModelYear} ({car.DailyPrice}$))");
            }
        }

        public List<Car> GetAll()
        {
            return _carDal.GetAll();
        }

        public List<CarDetailDto> GetCarDetails()
        {
            return _carDal.GetCarDetails();
        }

        public List<Car> GetCarsByBrandId(int brandId)
        {
            return _carDal.GetAll(c => c.BrandId == brandId);
        }

        public List<Car> GetCarsByColorId(int colorId)
        {
            return _carDal.GetAll(c => c.ColorId == colorId);
        }
    }
}
