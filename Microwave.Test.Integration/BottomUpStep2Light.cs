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
        private IOutput output;
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
        public void TurnOn_WasOff_CorrectOutput()
        {
            sut.TurnOn();
            Assert.That(stringWriter.ToString().Contains("turned on"));
        }

        [Test]
        public void TurnOff_WasOn_CorrectOutput()
        {
            sut.TurnOn();
            sut.TurnOff();
            Assert.That(stringWriter.ToString().Contains("turned off"));
        }

        [Test]
        public void TurnOn_WasOn_CorrectOutput()
        {
            sut.TurnOn();
            sut.TurnOn();
            Assert.That(stringWriter.ToString().Contains("turned on"));
        }

        [Test]
        public void TurnOff_WasOff_CorrectOutput()
        {
            sut.TurnOff();
            Assert.That(!stringWriter.ToString().Contains("turned off"));
        }
    }
}