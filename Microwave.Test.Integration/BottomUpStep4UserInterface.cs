﻿using System;
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
        public void Ready_DoorOpen_LightOn()
        {
            door.Open();

            Assert.That(stringWriter.ToString().Contains("turned on"));
        }

        [Test]
        public void DoorOpen_DoorClose_LightOff()
        {
            door.Open();
            door.Close();
            //light.TurnOff();

            //door.Closed += Raise.EventWith(this, EventArgs.Empty);
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
                powerButton.Press();
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
                powerButton.Press();
            }
            Assert.That(stringWriter.ToString().Contains("50"));
        }

        [Test]
        public void SetPower_CancelButton_DisplayCleared()
        {
            powerButton.Press();
            startCancelButton.Press();

            Assert.That(stringWriter.ToString().Contains("cleared"));
        }

        [Test]
        public void SetPower_DoorOpened_DisplayCleared()
        {
            powerButton.Press();
            door.Open();

            Assert.That(stringWriter.ToString().Contains("cleared"));
        }

        [Test]
        public void SetPower_DoorOpened_LightOn()
        {
            powerButton.Press();
            door.Open();

            Assert.That(stringWriter.ToString().Contains("Light is turned on"));
        }

        [Test]
        public void SetPower_TimeButton_TimeIs1()
        {
            powerButton.Press();
            timeButton.Press();

            Assert.That(stringWriter.ToString().Contains("1:00"));
        }

        [Test]
        public void SetPower_TimeButton_TimeIs2()
        {
            powerButton.Press();
            timeButton.Press();
            timeButton.Press();

            Assert.That(stringWriter.ToString().Contains("2:00"));
        }

        [Test]
        public void SetTime_StartButton_CookerIsCalled()
        {
            powerButton.Press();
            timeButton.Press();
            startCancelButton.Press();

            Assert.That(stringWriter.ToString().Contains("2:00"));
        }

        [Test]
        public void SetTime_DoorOpened_DisplayCleared()
        {
            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetPower
            timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetTime
            door.Opened += Raise.EventWith(this, EventArgs.Empty);

            display.Received().Clear();
        }

        [Test]
        public void SetTime_DoorOpened_LightOn()
        {
            powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetPower
            timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            // Now in SetTime
            door.Opened += Raise.EventWith(this, EventArgs.Empty);

            light.Received().TurnOn();
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

            //cooker.Received(1).StartCooking(100, 120);
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

            //cooker.Received(1).StartCooking(700, 60);

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

            light.Received(1).TurnOn();
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

            //uut.CookingIsDone();
            light.Received(1).TurnOff();
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

            // Cooking is done
            //uut.CookingIsDone();
            display.Received(1).Clear();
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

            //cooker.Received(1).Stop();
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

            display.Received(1).Clear();
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

            //cooker.Received(1).Stop();
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

            light.Received(1).TurnOff();
        }
    }
}
