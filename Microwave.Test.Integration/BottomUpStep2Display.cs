using System.IO;
using Microwave.Classes.Boundary;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class BottomUpStep2Display
    {
        private Display sut;
        private Output output;
        private StringWriter stringWriter;

        [SetUp]
        public void Setup()
        {
            output = Substitute.For<Output>();
            sut = new Display(output);
            stringWriter = new StringWriter();
        }

        [Test]
        public void displayShowsTimeCorrectly_ZeroMinZeroSec()
        {
            sut.ShowTime(0, 0);
            Assert.That(stringWriter.ToString().Contains("00:00"));
        }

        [Test]
        public void displayShowsTimeCorrectly_ZeroMinSomeSec()
        {
            sut.ShowTime(0, 7);
            Assert.That(stringWriter.ToString().Contains("00:07"));
        }

        [Test]
        public void displayShowsTimeCorrectly_SomeMinZeroSec()
        {
            sut.ShowTime(4, 0);
            Assert.That(stringWriter.ToString().Contains("04:00"));
        }

        [Test]
        public void displayShowsTimeCorrectly_SomeMinSomeSec()
        {
            sut.ShowTime(08, 05);
            Assert.That(stringWriter.ToString().Contains("08:05"));
        }

        [Test]
        public void displayShowsPowerCorrectly_Zero()
        {
            sut.ShowPower(0);
            Assert.That(stringWriter.ToString().Contains("0 W"));
        }

        [Test]
        public void displayShowsPowerCorrectly_NonZero()
        {
            sut.ShowPower(50);
            Assert.That(stringWriter.ToString().Contains("50 W"));
        }

        [Test]
        public void displayClearCorrectly()
        {
            sut.Clear();
            Assert.That(stringWriter.ToString().Contains("cleared"));
        }
    }
}