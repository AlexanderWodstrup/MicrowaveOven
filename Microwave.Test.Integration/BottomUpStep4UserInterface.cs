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
    public class BottomUpStep4UserInterface
    {
        private UserInterface sut;
        private IButton timeButton;
        private IButton powerButton;
        private IButton startCancelButton;
        private IDoor door;
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

            timeButton = Substitute.For<IButton>();
            powerButton = Substitute.For<IButton>();
            startCancelButton = Substitute.For<IButton>();
            door = Substitute.For<IDoor>();
            output = new Output();
            display = new Display(output);
            light = new Light(output);
            timer = new Timer();
            powerTube = new PowerTube(output);
            cookController = new CookController(timer, display, powerTube);

            sut = new UserInterface(powerButton, timeButton, startCancelButton, door, display, light, cookController);
        }

        [Test]
        public void Ready_DoorOpen_LightOn()
        {
            door.Opened += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(stringWriter.ToString().Contains("turned on"));
        }

        [Test]
        public void DoorOpen_DoorClose_LightOff()
        {
            door.Opened += Raise.EventWith(this, EventArgs.Empty);
            door.Closed += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(stringWriter.ToString().Contains("turned off"));
        }

        [Test]
        public void Ready_DoorOpenClose_Ready_PowerIs50()
        {
            door.Open();
            door.Close();
            powerTube.TurnOn(50);

            Assert.That(stringWriter.ToString().Contains("PowerTube works with 50"));
        }

        [Test]
        public void Ready_2PowerButton_PowerIs100()
        {
            powerTube.TurnOn(100);
            powerTube.TurnOff();

            Assert.That(stringWriter.ToString().Contains("PowerTube works with 100"));
        }

        [Test]
        public void Ready_14PowerButton_PowerIs700()
        {
            for (int i = 1; i <= 14; i++)
            {
                StringBuilder sb = stringWriter.GetStringBuilder();
                sb.Remove(0, sb.Length);
                powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            }
            Assert.That(stringWriter.ToString().Contains("700"));
        }

        [Test]
        public void Ready_15PowerButton_PowerIs50Again()
        {
            for (int i = 1; i <= 15; i++)
            {
                StringBuilder sb = stringWriter.GetStringBuilder();
                sb.Remove(0, sb.Length);
                powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            }
            Assert.That(stringWriter.ToString().Contains("50"));
        }

        [Test]
        public void SetPower_CancelButton_DisplayCleared()
        {
            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(stringWriter.ToString().Contains("cleared"));
        }

        [Test]
        public void SetPower_DoorOpened_DisplayCleared()
        {
            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            door.Opened += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(stringWriter.ToString().Contains("cleared"));
        }

        [Test]
        public void SetPower_DoorOpened_LightOn()
        {
            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            door.Opened += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(stringWriter.ToString().Contains("Light is turned on"));
        }

        [Test]
        public void SetPower_TimeButton_TimeIs1()
        {
            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(stringWriter.ToString().Contains("1:00"));
        }

        [Test]
        public void SetPower_TimeButton_TimeIs2()
        {
            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(stringWriter.ToString().Contains("2:00"));
        }

        [Test]
        public void SetTime_StartButton_CookerIsCalled()
        {
            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(stringWriter.ToString().Contains("1:00") && stringWriter.ToString().Contains("50 W"));
        }

        [Test]
        public void SetTime_DoorOpened_DisplayCleared()
        {
            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetPower
            timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetTime
            door.Opened += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(stringWriter.ToString().Contains("cleared"));
        }

        [Test]
        public void SetTime_DoorOpened_LightOn()
        {
            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetPower
            timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetTime
            door.Opened += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(stringWriter.ToString().Contains("Light is turned on"));
        }

        [Test]
        public void Ready_PowerAndTime_CookerIsCalledCorrectly()
        {
            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetPower
            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetTime
            timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            // Should call with correct values
            startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(stringWriter.ToString().Contains("2:00") && stringWriter.ToString().Contains("100 W"));
        }

        [Test]
        public void Ready_FullPower_CookerIsCalledCorrectly()
        {
            for (int i = 50; i <= 700; i += 50)
            {
                powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            }

            timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetTime

            // Should call with correct values
            startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(stringWriter.ToString().Contains("PowerTube works with 700"));

        }


        [Test]
        public void SetTime_StartButton_LightIsCalled()
        {
            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetPower
            timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetTime
            startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now cooking

            Assert.That(stringWriter.ToString().Contains("Light is turned on"));
        }

        [Test]
        public void Cooking_CookingIsDone_LightOff()
        {
            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetPower
            timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetTime
            startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in cooking

            sut.CookingIsDone();
            Assert.That(stringWriter.ToString().Contains("Light is turned off"));
        }

        [Test]
        public void Cooking_CookingIsDone_ClearDisplay()
        {
            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetPower
            timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetTime
            startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in cooking
            StringBuilder sb = stringWriter.GetStringBuilder();
            sb.Remove(0, sb.Length);
            // Cooking is done
            sut.CookingIsDone();
            Assert.That(stringWriter.ToString().Contains("Display cleared"));
        }

        [Test]
        public void Cooking_DoorIsOpened_CookerCalled()
        {
            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetPower
            timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetTime
            startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in cooking

            // Open door
            door.Opened += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(stringWriter.ToString().Contains("PowerTube turned off"));
        }

        [Test]
        public void Cooking_DoorIsOpened_DisplayCleared()
        {
            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetPower
            timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetTime
            startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in cooking

            // Open door
            door.Opened += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(stringWriter.ToString().Contains("Display cleared"));
        }

        [Test]
        public void Cooking_CancelButton_CookerCalled()
        {
            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetPower
            timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetTime
            startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in cooking

            // Open door
            startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(stringWriter.ToString().Contains("PowerTube turned off"));
        }

        [Test]
        public void Cooking_CancelButton_LightCalled()
        {
            

            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetPower
            timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetTime
            startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in cooking

            // Open door
            startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(stringWriter.ToString().Contains("Light is turned on"));
        }
    }
}
