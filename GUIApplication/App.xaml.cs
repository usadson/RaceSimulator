using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using Controller;

namespace GUIApplication;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        SetLanguage("it-IT");

        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

        base.OnStartup(e);
    }

    public static void SetLanguage(string name)
    {
        var vCulture = new CultureInfo(name);
        
        Thread.CurrentThread.CurrentCulture = vCulture;
        Thread.CurrentThread.CurrentUICulture = vCulture;
        CultureInfo.DefaultThreadCurrentCulture = vCulture;
        CultureInfo.DefaultThreadCurrentUICulture = vCulture;

        I18N.OnLanguageChanged();
    }
}
