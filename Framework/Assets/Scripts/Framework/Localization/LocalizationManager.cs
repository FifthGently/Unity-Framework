namespace Frameworks
{
    public class LocalizationManager
    {
        private static LanguageCache languageCache { get { return Singleton<LanguageCache>.Instance; } }

        public static string ShowText(string languageID)
        {
           return languageCache.ShowText(languageID);
        }

        public static void SetLanguage(LanguageEnum language)
        {
            languageCache.SetLanguage(language);
        }
    }
}
