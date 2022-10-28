using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Controller;
using Model;

namespace ControllerTest;

[TestFixture]
internal class Controller_TrackRegistry
{
    [SetUp]
    public void SetUp() => TrackRegistry.Initialize();

    [TearDown]
    public void TearDown() => TrackRegistry.Reset();

    [Test]
    public void VerifyNotEmpty()
    {
        Assert.That(TrackRegistry.All, Is.Not.Empty);
    }

    [Test]
    public void AllIsComplete()
    {
        Assert.That(TrackRegistry.All, Is.EquivalentTo(TrackRegistry.TrackByName.Values));

        List<Track> tracksInTracksByCup = new();
        foreach (var entry in TrackRegistry.TracksByCup)
            tracksInTracksByCup.AddRange(entry.Value);
        Assert.That(TrackRegistry.All, Is.EquivalentTo(tracksInTracksByCup));
    }
}