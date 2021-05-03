using Microwave.Classes.Boundary;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class BottomUpStep2PowerTube
    {
        private PowerTube sut;
        private IOutput fakeOutput;
        [SetUp]
        public void Setup()
        {
            fakeOutput = Substitute.For<IOutput>();
            sut = new PowerTube(fakeOutput);
        }
        [TestCase(1)]
        [TestCase(50)]
        [TestCase(100)]
        public void PowerTubeIsOff_TurnOn(int power)
        {
            sut.TurnOn(power);
            fakeOutput.Received().OutputLine(Arg.Is<string>(s => s.Contains($"{power}") && s.Contains("PowerTube works")));
        }

        [TestCase(-50)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(101)]
        [TestCase(150)]
        public void PowerTubeIsOn_AndPowerExcidesRange_ThrowsException(int power)
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => sut.TurnOn(power));
        }

        [Test]
        public void PowerTubeIsOn_TurnOffCorrectly()
        {
            sut.TurnOn(25);
            sut.TurnOff();
            fakeOutput.Received().OutputLine(Arg.Is<string>(str => str.Contains("turned off")));
        }

        [Test]
        public void PowerTubeIsOff_TurnOff_NoOutput()
        {
            sut.TurnOff();
            fakeOutput.Received(0).OutputLine(Arg.Any<string>());
        }

        [TestCase(50)]
        [TestCase(25)]
        [TestCase(10)]
        public void PowerTubeIsOn_TurnOn_ThrowsException(int power)
        {
            sut.TurnOn(25);
            Assert.Throws<System.ApplicationException>(() => sut.TurnOn(power));
        }
    }
}