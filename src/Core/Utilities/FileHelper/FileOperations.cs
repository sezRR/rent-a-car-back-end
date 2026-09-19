using Core.Utilities.Results;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Core.Utilities.FileOperations
{
    public static class FileOperations
    {
        public static DataResult<string> CheckCarImageDefaultOrNot(IFormFile file, string path)
        {
            if (file == null)
            {
                return new SuccessDataResult<string>("default.png","");
            }

            return new ErrorDataResult<string>(AddCarImage(file, path), "CarImage");
        }

        public static string AddCarImage(IFormFile file, string path)
        {
            var fileName = FileHelper.FileHelper.Add(file, ref path);
            return fileName;
        }
    }
}