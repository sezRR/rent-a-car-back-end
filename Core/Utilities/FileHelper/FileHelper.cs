using Core.Utilities.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Utilities.FileHelper
{
    public class FileHelper
    {
        public static IConfiguration Configuration { get; set; }

        public static string Add(IFormFile file, ref string path)
        {
            CheckPathValue(ref path);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var sourcePath = Path.GetTempFileName();
            if (file.Length > 0)
            {
                using var stream = new FileStream(sourcePath, FileMode.Create);
                file.CopyTo(stream);
            }

            var result = NewPath(file);
            File.Move(sourcePath, path+result);

            return result;
        }

        public static string Update(IFormFile file, string oldPath, ref string path)
        {
            //path = CheckPathValue(ref path);

            var newImagePath = Add(file, ref path);

            File.Delete(path+oldPath);

            return newImagePath;
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

        public static string CheckPathValue(ref string path)
        {
            path ??= Path.GetFullPath(Configuration["Paths:DefaultCarImagePath"]);
            return path;
        }
    }
}
