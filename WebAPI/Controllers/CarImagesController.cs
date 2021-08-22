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
        public IWebHostEnvironment WebHostEnvironment { get; set; }
        private readonly ICarImageService _carImageService;

        public CarImagesController(ICarImageService carImageService, IWebHostEnvironment webHostEnvironment)
        {
            _carImageService = carImageService;
            WebHostEnvironment = webHostEnvironment;
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
        public IActionResult Add([FromForm(Name = "carId")] string carId, [FromForm(Name = "file")] IFormFile file, string path = null)
        {
            var result = _carImageService.Add(new CarImage { CarId = Convert.ToInt32(carId), Date = DateTime.Now }, file, path);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("update")]
        public IActionResult Update([FromForm(Name = "objectFile")] IFormFile objectFile, [FromForm(Name = "carImage")] CarImage carImage, string path = null)
        {
            var result = _carImageService.Update(carImage, objectFile, path);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        [HttpDelete("delete")]
        public IActionResult Delete(int id, string path = null)
        {
            var carImages = _carImageService.GetByCarId(id).Data;

            if (carImages.Count == 0)
            {
                return BadRequest("This car do not have image!");
            }

            List<IResult> results = new();

            foreach (var carImage in carImages)
            {
                var result = _carImageService.Delete(carImage, path);
                results.Add(result);     
            }

            foreach (var result in results)
            {
                if (!result.Success)
                {
                    return BadRequest(result);
                }
            }

            return Ok(results[0]);
        }
    }
}
