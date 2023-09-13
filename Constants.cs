namespace AdrianTube2;

public static class Constants
{
    public static string StaticDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "static");
    public static string ThumbnailDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "static", "thumbnails");
    public static string VideoDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "static", "videos");
    public const double MaxVideoSize = 1024 * 1024 * 25;
    public const double MaxThumbnailSize = 1024 * 1024 * 5;
    public const int MaxShortVideoDuration = 60;
}