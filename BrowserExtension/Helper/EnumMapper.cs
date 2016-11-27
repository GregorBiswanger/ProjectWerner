using BrowserExtension.Enums;

namespace BrowserExtension.Helper
{
    public class EnumMapper
    {
        public static string MapLanguage(SearchLanguage searchLanguage)
        {
            switch (searchLanguage)
            {
                default:
                case SearchLanguage.German:
                    return "de-de";
                case SearchLanguage.AmericanEnglish:
                    return "en-us";
                case SearchLanguage.BritishEnglish:
                    return "en-gb";
            }
        }

        public static string MapSafeSearchFilter(SafeSearchFilter safeSearchFilter)
        {
            switch (safeSearchFilter)
            {
                default:
                case SafeSearchFilter.Moderate:
                    return "moderate";
                case SafeSearchFilter.Strict:
                    return "strict";
                case SafeSearchFilter.Off:
                    return "off";
            }
        }
    }
}
