using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageViewer.Services;

public interface IImageService
{
    IEnumerable<string> GetImagesInFolder(string folderPath);
}

public class ImageService : IImageService
{
    private static readonly string[] SupportExtensions = { 
        ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff", ".ico", // Standard
        ".webp", ".heic", ".svg", ".psd", ".avif", ".tga", // Advanced
        ".cr2", ".nef", ".dng", ".arw", ".orf" // RAW
    };

    public IEnumerable<string> GetImagesInFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            System.Windows.MessageBox.Show($"Directory does not exist: {folderPath}");
            return Enumerable.Empty<string>();
        }

        try 
        {
            var options = new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = false };
            var files = Directory.EnumerateFiles(folderPath, "*.*", options)
                .Where(file => SupportExtensions.Contains(Path.GetExtension(file).ToLowerInvariant()));

            return files;
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Error reading folder: {ex.Message}");
            return Enumerable.Empty<string>();
        }
    }
}
