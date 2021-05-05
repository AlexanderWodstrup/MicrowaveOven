using System;
using System.IO;
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
        private Output output;
        private StringWriter stringWriter;
        [SetUp]
        public void Setup()
        {
            stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            output = new Output();
            
            sut = new PowerTube(output);
        }
        
        [TestCase(50)]
        [TestCase(100)]
        [TestCase(700)]
        public void PowerTubeIsOff_TurnOn(int power)
        {
            sut.TurnOn(power);
            Assert.That(stringWriter.ToString().Contains($"{power}") && stringWriter.ToString().Contains("PowerTube works"));
        }

        [TestCase(-750)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(701)]
        [TestCase(750)]
        public void PowerTubeIsOn_AndPowerExcidesRange_ThrowsException(int power)
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => sut.TurnOn(power));
        }

        [Test]
        public void PowerTubeIsOn_TurnOffCorrectly()
        {
            sut.TurnOn(50);
            sut.TurnOff();
            Assert.That(stringWriter.ToString().Contains("turned off"));
        }

        [Test]
        public void PowerTubeIsOff_TurnOff_NoOutput()
        {
            sut.TurnOff();
            Assert.That(!stringWriter.ToString().Contains("turned off"));
            
        }

        [TestCase(50)]
        [TestCase(100)]
        [TestCase(700)]
        public void PowerTubeIsOn_TurnOn_ThrowsException(int power)
        {
            sut.TurnOn(50);
            Assert.Throws<System.ApplicationException>(() => sut.TurnOn(power));
        }
    }
}