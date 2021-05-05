using System;
using System.IO;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class BottomUpStep2CookController
    {
        private StringWriter stringWriter;

        private CookController sut;
        private Display display;
        private PowerTube powerTube;
        private Output output;
        private Timer timer;
        private IUserInterface ui;
        [SetUp]
        public void Setup()
        {
            stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            ui = Substitute.For<IUserInterface>();
            output = new Output();
            timer = new Timer();
            display = new Display(output);
            powerTube = new PowerTube(output);


            sut = new CookController(timer,display,powerTube, ui);
        }
        [Test]
        public void StartCooking_ValidParameters_TimerStarted()
        {
            sut.StartCooking(50, 60);

            Assert.AreEqual(timer.TimeRemaining, 60);
            
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
            sut.StartCooking(50, 60*60);
            timer.TimerTick += Raise.EventWith(timer, EventArgs.Empty);
            timer.TimerTick += Raise.EventWith(timer, EventArgs.Empty);
            timer.TimerTick += Raise.EventWith(timer, EventArgs.Empty);
            timer.TimerTick += Raise.EventWith(timer, EventArgs.Empty);
            Assert.That(stringWriter.ToString().Contains("0:59"));
        }

        [Test]
        public void Cooking_TimerExpired_PowerTubeOff()
        {
            sut.StartCooking(50, 60);

            timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            powerTube.Received().TurnOff();
        }

        [Test]
        public void Cooking_TimerExpired_UICalled()
        {
            sut.StartCooking(50, 60);

            timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            ui.Received().CookingIsDone();
        }

        [Test]
        public void Cooking_Stop_PowerTubeOff()
        {
            sut.StartCooking(50, 60);
            sut.Stop();

            powerTube.Received().TurnOff();
        }
    }
}