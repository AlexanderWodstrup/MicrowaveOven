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
    public class BottomUpStep2Light
    {
        private Light sut;
        private Output output;
        private StringWriter stringWriter;
        [SetUp]
        public void Setup()
        {
            stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            
            output = new Output();
            sut = new Light(output);
        }

        [Test]
        public void TurnOnLightWorksCorrectly()
        {
            sut.TurnOn();
            Assert.That(stringWriter.ToString().Contains("turned on"));
            //output.Received(1).OutputLine(Arg.Is<string>(s => s.Contains("turned on")));
        }

        [Test]
        public void TurnOffLightWorksCorrectly()
        {
            sut.TurnOn();
            sut.TurnOff();
            Assert.That(stringWriter.ToString().Contains("turned off"));
            //output.Received(1).OutputLine(Arg.Is<string>(s => s.Contains("turned off")));
        }
    }
}