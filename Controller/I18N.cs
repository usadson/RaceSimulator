using System.Globalization;
using System.Resources;

namespace Controller;

public static class I18N
{
    private static ResourceManager? _resourceManagerImpl;
    private static CultureInfo? _cultureInfoImpl;

    public static bool IsInitialized => _resourceManagerImpl != null && _cultureInfoImpl != null;

    private static CultureInfo TheCultureInfo
    {
        get => _cultureInfoImpl!;
        set => _cultureInfoImpl = value;
    }

    private static ResourceManager TheResourceManager
    {
        get => _resourceManagerImpl!;
        set => _resourceManagerImpl = value;
    }

    public static void Initialize()
    {
        TheResourceManager = Properties.Resources.ResourceManager;
        OnLanguageChanged();
    }

    public static void Reset()
    {
        _resourceManagerImpl = null;
        _cultureInfoImpl = null;
    }

    public static void OnLanguageChanged()
    {
        TheCultureInfo = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
    }

    public static string Translate(string input) => TheResourceManager.GetString(input, TheCultureInfo) ?? input;
}