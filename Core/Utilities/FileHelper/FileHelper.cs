using Core.Utilities.Results;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Utilities.FileHelper
{
    public class FileHelper
    {
        public static string Add(IFormFile file, string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var sourcePath = Path.GetTempFileName();
            if (file.Length > 0)
            {
                using (var stream = new FileStream(sourcePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }

            var result = NewPath(file);
            File.Move(sourcePath, path+result);

            return result;
        }

        public static string Update(IFormFile file, string sourcePath, string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
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

        public static IResult Delete(string fileName, string path)
        {
            var sourcePath = path + fileName;
            File.Delete(sourcePath);

            return new SuccessResult();
        }

        public static string NewPath(IFormFile file)
        {
            FileInfo fileInfo = new FileInfo(file.FileName);
            string fileExtension = fileInfo.Extension;

            var uniqueFileName = 
                Guid.NewGuid().ToString("N") +
                fileExtension;

            string result = $@"{uniqueFileName}";

            return result;
        }
    }
}
