using System;
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
            output = new Output();
            sut = new Display(output);
            stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
        }

        [Test]
        public void DisplayShowsTimeCorrectly_ZeroMinZeroSec()
        {
            sut.ShowTime(0, 0);
            Assert.That(stringWriter.ToString().Contains("00:00"));
        }

        [Test]
        public void DisplayShowsTimeCorrectly_ZeroMinSomeSec()
        {
            sut.ShowTime(0, 7);
            Assert.That(stringWriter.ToString().Contains("00:07"));
        }

        [Test]
        public void DisplayShowsTimeCorrectly_SomeMinZeroSec()
        {
            sut.ShowTime(4, 0);
            Assert.That(stringWriter.ToString().Contains("04:00"));
        }

        [Test]
        public void DisplayShowsTimeCorrectly_SomeMinSomeSec()
        {
            sut.ShowTime(08, 05);
            Assert.That(stringWriter.ToString().Contains("08:05"));
        }

        [Test]
        public void DisplayShowsPowerCorrectly_Zero()
        {
            sut.ShowPower(0);
            Assert.That(stringWriter.ToString().Contains("0 W"));
        }

        [Test]
        public void DisplayShowsPowerCorrectly_NonZero()
        {
            sut.ShowPower(50);
            Assert.That(stringWriter.ToString().Contains("50 W"));
        }

        [Test]
        public void DisplayClearCorrectly()
        {
            sut.Clear();
            Assert.That(stringWriter.ToString().Contains("cleared"));
        }
    }
}