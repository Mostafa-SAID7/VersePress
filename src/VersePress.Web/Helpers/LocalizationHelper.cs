using System.Globalization;

namespace VersePress.Web.Helpers;

public static class LocalizationHelper
{
    public static string GetLocalizedString(string englishValue, string arabicValue)
    {
        var currentCulture = CultureInfo.CurrentCulture.Name;
        return currentCulture == "ar-SA" ? arabicValue : englishValue;
    }

    public static bool IsRtl()
    {
        var currentCulture = CultureInfo.CurrentCulture.Name;
        return currentCulture == "ar-SA";
    }

    public static string GetCurrentCulture()
    {
        return CultureInfo.CurrentCulture.Name;
    }
}
