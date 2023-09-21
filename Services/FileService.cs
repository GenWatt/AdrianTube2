using AdrianTube2.Services.Interfaces;
using Microsoft.AspNetCore.Components.Forms;

namespace AdrianTube2.Services;

public class FileService : IFileService {
    public double ConvertSizeToUnit(double bytes, string unit) {
        var lowerUnit = unit.ToLower();

        if (lowerUnit == "mb") {
            var mb = (double)bytes / (1024 * 1024);
            return Math.Round(mb, 1);
        } else if (lowerUnit == "gb") {
            var gb = (double)bytes / (1024 * 1024 * 1024);
            return Math.Round(gb, 1) ;
        } else if (lowerUnit == "b") {
            return bytes;
        } else {
            throw new Exception("Invalid unit");
        }
    }

    public bool IsImage(IBrowserFile file) {
        var imageTypes = new List<string> {
            "image/png",
            "image/jpeg",
            "image/gif",
            "image/bmp",
            "image/webp",
            "image/svg+xml"
        };

        return imageTypes.Contains(file.ContentType);
    }

    public async Task<string> CreateImageUrl(IBrowserFile file) {
        if (!IsImage(file)) return ""; 

        using Stream imageStream = file.OpenReadStream((long)Constants.MaxThumbnailSize);
        
        using MemoryStream ms = new();
        await imageStream.CopyToAsync(ms);

        return $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";
    }

    public async Task<string> CreateVideoUrl(IBrowserFile file) {
        using Stream videoStream = file.OpenReadStream((long)Constants.MaxVideoSize);
        
        using MemoryStream ms = new();
        await videoStream.CopyToAsync(ms);

        return $"data:video/mp4;base64,{Convert.ToBase64String(ms.ToArray())}";
    }

    public void CreateDirectory(string path) {
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
    }

    public void DeleteFileIfExists(string path) {
        if (File.Exists(path)) {
            File.Delete(path);
        }
    }
}