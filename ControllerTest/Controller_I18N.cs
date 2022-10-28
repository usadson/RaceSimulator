using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Controller;

namespace ControllerTest;

[TestFixture]
internal class Controller_I18N
{
    [TearDown]
    public void TearDown() => I18N.Reset();

    [Test]
    public void IsInitializedTest()
    {
        Assert.That(I18N.IsInitialized, Is.False);
    }

    [Test]
    public void InitializeTest()
    {
        Assert.That(I18N.IsInitialized, Is.False);
        I18N.Initialize();
        Assert.That(I18N.IsInitialized, Is.True);
        I18N.Reset();
    }

    [Test]
    public void TranslateStringUnknown()
    {
        Assert.That(I18N.IsInitialized, Is.False);
        I18N.Initialize();

        var stringToTranslate = "[TEST} String that does not exist";
        var data = I18N.Translate(stringToTranslate);
        Assert.That(stringToTranslate, Is.SameAs(data));

        Assert.That(I18N.IsInitialized, Is.True);
        I18N.Reset();
    }
}