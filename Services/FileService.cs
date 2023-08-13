using Microsoft.AspNetCore.Components.Forms;

namespace AdrianTube2.Services;

public class FileService {
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

    public void CreateDirectory(string path) {
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
    }
}