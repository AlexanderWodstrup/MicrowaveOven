using System;
using System.Runtime.Serialization;
using System.IO;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class BottomUpStep3UserInterface
    {
        private UserInterface sut;
        private Button timeButton;
        private Button powerButton;
        private Button startCancelButton;
        private Door door;
        private Display display;
        private Light light;
        private CookController cookController;
        private Output output;
        //private IOutput output;
        private Timer timer;
        private PowerTube powerTube;
        private StringWriter stringWriter;

        [SetUp]
        public void Setup()
        {
            stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            timeButton = new Button();
            powerButton = new Button();
            startCancelButton = new Button();
            door = new Door();
            output = new Output();
            //output = Substitute.For<IOutput>();
            display = new Display(output);
            light = new Light(output);
            timer = new Timer();
            powerTube = new PowerTube(output);
            cookController = new CookController(timer, display, powerTube);

            sut = new UserInterface(powerButton, timeButton, startCancelButton, door, display, light, cookController);
        }

        [Test]
        public void LightIsTurnedOn()
        {
            //light.TurnOn();
            door.Open();

            //door.Opened += Raise.EventWith(this, EventArgs.Empty);
            Assert.That(stringWriter.ToString().Contains("turned on"));
        }

        [Test]
        public void LightIsTurnedOff()
        {
            door.Open();
            door.Close();
            //light.TurnOff();

            //door.Closed += Raise.EventWith(this, EventArgs.Empty);
            Assert.That(stringWriter.ToString().Contains("turned off"));
        }

        [TestCase(50)]
        [TestCase(350)]
        [TestCase(700)]
        public void DoorOpenClose_Powered80W(int power)
        {
            powerTube.TurnOn(power);

            Assert.That(stringWriter.ToString().Contains($"PowerTube works with {power}\r\n"));
        }
    }
}
