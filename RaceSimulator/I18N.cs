using System.Globalization;
using System.Resources;

namespace RaceSimulator
{
    internal class I18N
    {
        static ResourceManager resourceManager;
        static CultureInfo cultureInfo;

        public static void Initialize()
        {
            cultureInfo = CultureInfo.CreateSpecificCulture("it");
            resourceManager = Properties.Resources.ResourceManager;
        }

        public static string Translate(string input)
        {
            var str = resourceManager.GetString(input, cultureInfo);
            if (str != null)
                return str;
            return input;
        }
    }
}
