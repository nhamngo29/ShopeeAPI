using Microsoft.AspNetCore.Http;
using Shopee.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System;
using Shopee.Application.Common.Exceptions;

namespace Shopee.Infrastructure.Services;
public class LocalStorageFileService : IFileService
{
    private readonly IWebHostEnvironment _environment;

    public LocalStorageFileService(IWebHostEnvironment environment)
    {
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string[] allowedFileExtensions)
    {
        if (fileStream == null)
            throw new ArgumentNullException(nameof(fileStream));

        var ext = Path.GetExtension(fileName);
        if (!allowedFileExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"Only {string.Join(", ", allowedFileExtensions)} are allowed.");

        var uploadPath = Path.Combine(_environment.ContentRootPath, "wwwroot","images");

        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        var uniqueFileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadPath, uniqueFileName);

        using var fileStreamOutput = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fileStreamOutput);

        return uniqueFileName; // Trả về tên file duy nhất
    }

    public void DeleteFile(string fileNameWithExtension)
    {
        if (string.IsNullOrEmpty(fileNameWithExtension))
            throw new ArgumentNullException(nameof(fileNameWithExtension));

        var filePath = Path.Combine(_environment.ContentRootPath, "Uploads", fileNameWithExtension);
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File {fileNameWithExtension} does not exist.");

        File.Delete(filePath);
    }

    public async Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions)
    {
        if (imageFile == null)
        {
            throw new ArgumentNullException(nameof(imageFile));
        }

        var contentPath = _environment.ContentRootPath;
        var path = Path.Combine(contentPath,"wwwroot", "images");
        // path = "c://projects/ImageManipulation.Ap/uploads" ,not exactly, but something like that

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        // Check the allowed extenstions
        var ext = Path.GetExtension(imageFile.FileName);
        if (!allowedFileExtensions.Contains(ext))
        {
            throw new BadRequestException($"Only {string.Join(",", allowedFileExtensions)} are allowed.");
        }

        // generate a unique filename
        var fileName = $"{Guid.NewGuid().ToString()}{ext}";
        var fileNameWithPath = Path.Combine(path, fileName);
        using var stream = new FileStream(fileNameWithPath, FileMode.Create);
        await imageFile.CopyToAsync(stream);
        return fileName;
    }
}
