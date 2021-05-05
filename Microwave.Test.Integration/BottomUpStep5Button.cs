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
            userInterface = new UserInterface(powerButton, timerButton, startcancelButton, door, display, light,
                cookController);
            cookController.UI = userInterface;

        }

        [TestCase(2)]
        [TestCase(5)]
        [TestCase(9)]
        [TestCase(14)]
        public void PowerButton_Ready_PowerShows(int numPressed)
        {
            for (int i = 0; i < numPressed; i++)
            {
                powerButton.Press();
            }


            string power = (numPressed * 50).ToString();
            Assert.That(stringWriter.ToString().Contains(power));
        }

        [Test]
        public void PowerButton_Ready_PowerShows_ResetTo50()
        {
            for (int i = 0; i < 15; i++)
            {
                powerButton.Press();
            }

            Assert.That(stringWriter.ToString().Contains("50"));
        }

        [TestCase(2)]
        [TestCase(5)]
        [TestCase(9)]
        [TestCase(100)]
        public void TimeButtom_ShowTime(int numPressed)
        {
            powerButton.Press();
            for (int i = 0; i < numPressed; i++)
            {
                timerButton.Press();
            }

            string time = numPressed.ToString();

            Assert.That(stringWriter.ToString().Contains(time));
        }

        //[Test]
        //public void PowerButton_
    }
}
