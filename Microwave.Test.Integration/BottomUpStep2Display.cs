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
        private IOutput output;

        [SetUp]
        public void Setup()
        {
            output = Substitute.For<IOutput>();
            sut = new Display(output);
        }

        [Test]
        public void displayShowsTimeCorrectly_ZeroMinZeroSec()
        {
            sut.ShowTime(0, 0);
            output.Received().OutputLine(Arg.Is<string>(str=>str.Contains("00:00")));
        }

        [Test]
        public void displayShowsTimeCorrectly_ZeroMinSomeSec()
        {
            sut.ShowTime(0, 7);
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains("00:07")));
        }

        [Test]
        public void displayShowsTimeCorrectly_SomeMinZeroSec()
        {
            sut.ShowTime(4, 0);
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains("04:00")));
        }

        [Test]
        public void displayShowsTimeCorrectly_SomeMinSomeSec()
        {
            sut.ShowTime(08, 05);
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains("08:05")));
        }

        [Test]
        public void displayShowsPowerCorrectly_Zero()
        {
            sut.ShowPower(0);
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains("0 W")));
        }

        [Test]
        public void displayShowsPowerCorrectly_NonZero()
        {
            sut.ShowPower(50);
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains("50 W")));
        }

        [Test]
        public void displayClearCorrectly()
        {
            sut.Clear();
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains("cleared")));
        }
    }
}