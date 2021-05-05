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
    class BottomUpStep3Button
    {
        private UserInterface userInterface;
        private CookController cookController;
        private Display display;
        private Light light;
        private Output output;
        private Door door;
        private Timer timer;
        private PowerTube powerTube;
        private Button powerButton;
        private Button timerButton;
        private Button startcancelButton;
        private StringWriter stringWriter;

        [SetUp]
        public void Setup()
        {
            stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            output = new Output();
            display = new Display(output);
            light = new Light(output);
            door = new Door();
            timer = new Timer();
            powerButton = new Button();
            timerButton = new Button();
            startcancelButton = new Button();
            powerTube = new PowerTube(output);
            cookController = new CookController(timer, display, powerTube);
            userInterface = new UserInterface(powerButton, timerButton, startcancelButton, door, display, light, cookController);
            cookController.UI = userInterface;

        }

        [Test]
        public void PowerButton_Ready_PowerShows(int power)
        {
            
            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(stringWriter.ToString().Contains("Display shows "));
        }
    }
}
