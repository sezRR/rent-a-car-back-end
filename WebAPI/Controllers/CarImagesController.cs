using Business.Abstract;
using Core.Utilities.FileHelper;
using Core.Utilities.Results;
using Entities.Concrete;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarImagesController : ControllerBase
    {
        ICarImageService _carImageService;

        public CarImagesController(ICarImageService carImageService)
        {
            _carImageService = carImageService;
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var result = _carImageService.GetAll();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getbyid")]
        public IActionResult GetById(int id)
        {
            var result = _carImageService.GetById(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getbycarid")]
        public IActionResult GetByCarId(int carId)
        {
            var result = _carImageService.GetByCarId(carId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("add")]
        public IActionResult Add([FromForm(Name = "file")] IFormFile[] files, [FromForm] CarImage carImage)
        {
            List<IResult> errorResults = new List<IResult>();
            IResult result;

            foreach (var file in files)
            {
                CarImage tempCarImage = new CarImage(); // for multiple addition
                tempCarImage.CarId = carImage.CarId;
                tempCarImage.ImagePath = carImage.ImagePath;
                tempCarImage.Date = carImage.Date;

                result = _carImageService.Add(tempCarImage, file);

                if (!result.Success)
                {
                    errorResults.Add(result);
                }
            }

            if (errorResults.Count > 0)
            {
                return BadRequest(errorResults[0]);
            }

            return Ok();
        }

        [HttpPost("update")]
        public IActionResult Update([FromForm] IFormFile objectFile, CarImage carImage)
        {
            var result = _carImageService.Update(carImage, objectFile);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        [HttpPost("delete")]
        public IActionResult Delete([FromForm] int id)
        {
            var carImage = _carImageService.GetById(id).Data;

            var result = _carImageService.Delete(carImage);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
