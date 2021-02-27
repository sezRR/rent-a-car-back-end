using Core.Utilities.Results;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Core.Utilities.FileHelper
{
    public class FileHelper
    {
        public static string Add(IFormFile file)
        {
            var sourcePath = Path.GetTempFileName();
            if (file.Length > 0)
            {
                using (var stream = new FileStream(sourcePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }

            var result = NewPath(file);
            File.Move(sourcePath, result);

            return result;
        }

        public static string Update(IFormFile file, string sourcePath)
        {
            var result = NewPath(file);

            if (sourcePath.Length > 0)
            {
                using (var stream = new FileStream(result, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }

            File.Delete(sourcePath);

            return result;
        }

        public static IResult Delete(string sourcePath)
        {
            var file = Directory.GetFiles(sourcePath).Length;

            if (file == 0)
            {
                return new ErrorResult();
            }

            File.Delete(sourcePath);

            return new SuccessResult();
        }

        public static string NewPath(IFormFile file)
        {
            FileInfo fileInfo = new FileInfo(file.FileName);
            string fileExtension = fileInfo.Extension;

            var uniqueFileName = 
                DateTime.Now.Month + "-" +
                DateTime.Now.Day + "-" + 
                DateTime.Now.Year + "-" + 
                Guid.NewGuid().ToString("N") +
                fileExtension;

            string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName + @"\CarImages");

            string result = $@"{path}\{uniqueFileName}";

            return result;
        }
    }
}
