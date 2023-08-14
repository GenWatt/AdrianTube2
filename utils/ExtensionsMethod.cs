namespace AdrianTube2.utils;

static public class ExtensionsMethod
{
    static public string Truncate(this string str, int maxLength)
    {
        if (string.IsNullOrEmpty(str)) return str;
        return str.Substring(0, Math.Min(str.Length, maxLength)) + "...";
    }

    static public string GetTimeFromNow(this DateTime dateTime)
    {
        TimeSpan timeSpan = DateTime.Now.Subtract(dateTime);
        if (timeSpan.TotalDays > 365)
        {
            return $"{(int)(timeSpan.TotalDays / 365)} year(s) ago";
        }
        else if (timeSpan.TotalDays > 30)
        {
            return $"{(int)(timeSpan.TotalDays / 30)} month(s) ago";
        }
        else if (timeSpan.TotalDays > 7)
        {
            return $"{(int)(timeSpan.TotalDays / 7)} week(s) ago";
        }
        else if (timeSpan.TotalDays > 1)
        {
            return $"{(int)timeSpan.TotalDays} day(s) ago";
        }
        else if (timeSpan.TotalHours > 1)
        {
            return $"{(int)timeSpan.TotalHours} hour(s) ago";
        }
        else if (timeSpan.TotalMinutes > 1)
        {
            return $"{(int)timeSpan.TotalMinutes} minute(s) ago";
        }
        else
        {
            return $"{(int)timeSpan.TotalSeconds} second(s) ago";
        }
    }
}