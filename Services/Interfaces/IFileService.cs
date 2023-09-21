using Microsoft.AspNetCore.Components.Forms;

namespace AdrianTube2.Services.Interfaces;

public interface IFileService
{
    public double ConvertSizeToUnit(double bytes, string unit);

    public bool IsImage(IBrowserFile file);

    public Task<string> CreateImageUrl(IBrowserFile file);

    public Task<string> CreateVideoUrl(IBrowserFile file);

    public void CreateDirectory(string path);
    public void DeleteFileIfExists(string path);
}