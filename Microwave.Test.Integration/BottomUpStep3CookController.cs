﻿using System;
using System.Runtime.Serialization;
using System.IO;
using System.Threading;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Timer = Microwave.Classes.Boundary.Timer;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class BottomUpStep3CookController
    {
        private StringWriter stringWriter;
        private CookController sut;
        private IDisplay display;
        private IPowerTube powerTube;
        private IOutput output;
        private ITimer timer;
        private IUserInterface ui;
        [SetUp]
        public void Setup()
        {
            ui = Substitute.For<IUserInterface>();
            output = new Output();
            timer = new Timer();
            display = new Display(output);
            powerTube = new PowerTube(output);


            sut = new CookController(timer,display,powerTube, ui);
            stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
        }
        [Test]
        public void StartCooking_ValidParameters_TimerStarted()
        {
            sut.StartCooking(50, 60);
            
            Assert.AreEqual(timer.TimeRemaining, 60000);
            
        }

        [Test]
        public void StartCooking_ValidParameters_PowerTubeStarted()
        {
            sut.StartCooking(50, 60);

            Assert.That(stringWriter.ToString().Contains("works") && stringWriter.ToString().Contains("50"));
        }

        [Test]
        public void Cooking_TimerTick_DisplayCalled()
        {
            sut.StartCooking(50, 120);

            Thread.Sleep(5100);

            Assert.That(stringWriter.ToString().Contains("1:55"));
        }

        [Test]
        public void Cooking_TimerExpired_PowerTubeOff()
        {
            sut.StartCooking(50, 3);

            Thread.Sleep(3100);

            Assert.That(stringWriter.ToString().Contains("turned off"));
        }

        [Test]
        public void Cooking_TimerExpired_UICalled()
        {
            sut.StartCooking(50, 3);

            Thread.Sleep(3100);

            ui.Received().CookingIsDone();
        }

        [Test]
        public void Cooking_Stop_PowerTubeOff()
        {
            sut.StartCooking(50, 60);
            sut.Stop();
            Assert.That(stringWriter.ToString().Contains("turned off"));
        }

        [TestCase(50, 10)]
        [TestCase(350, 10)]
        [TestCase(700, 10)]
        public void StartCooking_StartsCooking(int power, int time)
        {
            sut.StartCooking(power, time);

            Assert.That(stringWriter.ToString().Contains($"PowerTube works with {power}\r\n"));
            //Assert.That(timer.TimeRemaining, Is.EqualTo(time));
        }

        [TestCase(40, 10)]
        [TestCase(710, 10)]
        public void StartCooking_InvalidPower_Throws(int power, int time)
        {
            Assert.That(() => sut.StartCooking(power, time), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(timer.TimeRemaining, Is.EqualTo(0));
        }

        [TestCase(200, 10)]
        public void StartCooking_AlreadyActive_Throws(int power, int time)
        {
            sut.StartCooking(power, time);

            Assert.That(() => sut.StartCooking(power, time), Throws.InstanceOf<ApplicationException>());
        }

        [Test]
        public void TurnOff_Active_DoesNothing()
        {
            sut.Stop();

            Assert.That(stringWriter.ToString().Contains(""));
        }

        [Test]
        public void TurnOff_Active_DisplaysSomething()
        {
            sut.StartCooking(100, 10);
            sut.Stop();

            Assert.That(stringWriter.ToString().Contains("turned off"));
        }
    }
}