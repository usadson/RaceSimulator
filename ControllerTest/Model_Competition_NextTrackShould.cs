using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model;

namespace ControllerTest
{
    [TestFixture]
    public class Model_Competition_NextTrackShould
    {
        private Competition _competition;

        [SetUp]
        public void SetUp()
        {
            _competition = new();
        }

        [Test]
        public void NextTrack_EmptyQueue_ReturnNull()
        {
            var result = _competition.NextTrack();
            Assert.That(result, Is.Null);
        }

        [Test]
        public void NextTrack_OneInQueue_ReturnTrack()
        {
            var track = new Track(Cup.Mushroom, "Luigi Circuit", Array.Empty<Section>());
            
            Assert.That(_competition.NextTrack(), Is.Null);
            _competition.Tracks.Enqueue(track);

            var result = _competition.NextTrack();
            Assert.That(result, Is.Not.Null);
            Assert.That(track, Is.EqualTo(result));
        }

        [Test]
        public void NextTrack_OneInQueue_RemoveTrackFromQueue()
        {
            var track = new Track(Cup.Banana, "DS Delfino Square", Array.Empty<Section>());
            
            Assert.That(_competition.NextTrack(), Is.Null);
            _competition.Tracks.Enqueue(track);

            var result = _competition.NextTrack();
            Assert.That(result, Is.Not.Null);
            Assert.That(track, Is.EqualTo(result));
            
            Assert.That(_competition.NextTrack(), Is.Null);
        }

        [Test]
        public void NextTrack_TwoInQueue_ReturnNextTrack()
        {
            var track1 = new Track(Cup.Special, "Rainbow Road", Array.Empty<Section>());
            var track2 = new Track(Cup.Leaf, "DS Desert Hills", Array.Empty<Section>());
            
            Assert.That(_competition.NextTrack(), Is.Null);
            
            _competition.Tracks.Enqueue(track1);
            _competition.Tracks.Enqueue(track2);
            
            Assert.That(track1, Is.EqualTo(_competition.NextTrack()));
            Assert.That(track2, Is.EqualTo(_competition.NextTrack()));
            
            Assert.That(_competition.NextTrack(), Is.Null);
        }
    }
}
