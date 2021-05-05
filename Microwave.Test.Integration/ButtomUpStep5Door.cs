using System;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class ButtomUpStep5Door
    {
        private Door door;
        private Display display;
        private Light light;
        private Button powerButton;
        private Button timeButton;
        private Button startcancelButton;
        private Output output;
        private UserInterface userInterface;
        private CookController cookController;
        private StringWriter stringWriter;
        private Timer timer;
        private PowerTube powerTube;

        [SetUp]
        public void Setup()
        {
            door = new Door();
            output = new Output();
            light = new Light(output);
            display = new Display(output);
            powerButton = new Button();
            timeButton = new Button();
            powerTube = new PowerTube(output);
            timer = new Timer();
            startcancelButton = new Button();
            cookController = new CookController(timer, display, powerTube);
            userInterface = new UserInterface(powerButton, timeButton, startcancelButton, door, display, light, cookController);
            stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
        }

        [Test]
        public void EventOpen_Ready_LightOn()
        {
            door.Open();

            Assert.That(stringWriter.ToString().Contains("Light is turned on"));
        }

        [Test]
        public void CookingIsOn_DoorOpens()
        {
            powerButton.Press();
            timeButton.Press();
            startcancelButton.Press();
            door.Open();
            Assert.That(stringWriter.ToString().Contains("Light is turned on") && stringWriter.ToString().Contains("PowerTube turned off") && stringWriter.ToString().Contains("Display cleared"));
        }


        [Test]
        public void EventOpen_ReadyOpen_LightOff()
        {
            door.Open();
            door.Close();

            Assert.That(stringWriter.ToString().Contains("Light is turned off"));
        }

        [Test]
        public void EventOpen_StartCooking_LightOn()
        {
           powerButton.Press();
           timeButton.Press();
           startcancelButton.Press();
           StringBuilder sb = stringWriter.GetStringBuilder();
           sb.Remove(0, sb.Length);

           door.Open();

           Assert.That(!stringWriter.ToString().Contains("Light is turned on")); // Der skal ikke printes at lyset bliver tændt eftersom det er tændt i forvejen fordi microovnen er i gang.
           Assert.That(stringWriter.ToString().Contains("PowerTube turned off") && stringWriter.ToString().Contains("Display cleared"));
        }
    }
}
