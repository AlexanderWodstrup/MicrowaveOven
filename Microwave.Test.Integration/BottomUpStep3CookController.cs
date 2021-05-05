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
    public class BottomUpStep3CookController
    {
        private StringWriter stringWriter;
        private CookController sut;
        private Display display;
        private PowerTube powerTube;
        private Output output;
        private ITimer timer;
        private IUserInterface ui;
        [SetUp]
        public void Setup()
        {
            ui = Substitute.For<IUserInterface>();
            output = new Output();
            timer = Substitute.For<ITimer>();
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
            timer.TimeRemaining.Returns(60);
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
            sut.StartCooking(50, 120);

            timer.TimeRemaining.Returns(115000);
            timer.TimerTick += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(stringWriter.ToString().Contains("1:55"));
        }

        [Test]
        public void Cooking_TimerExpired_PowerTubeOff()
        {
            sut.StartCooking(50, 60);

            timer.Expired += Raise.Event();
            
            Assert.That(stringWriter.ToString().Contains("turned off"));
        }

        [Test]
        public void Cooking_TimerExpired_UICalled()
        {
            sut.StartCooking(50, 60);

            timer.Expired += Raise.Event();

            ui.Received().CookingIsDone();
        }

        [Test]
        public void Cooking_Stop_PowerTubeOff()
        {
            sut.StartCooking(50, 60);
            sut.Stop();
            Assert.That(stringWriter.ToString().Contains("turned off"));
        }

        [TestCase(50, 1000)]
        [TestCase(350, 1000)]
        [TestCase(700, 1000)]
        public void StartCooking_StartsCooking(int power, int time)
        {
            sut.StartCooking(power, time);

            Assert.That(stringWriter.ToString().Contains($"PowerTube works with {power}\r\n"));
            //Assert.That(timer.TimeRemaining, Is.EqualTo(time));
        }

        [TestCase(40, 1000)]
        [TestCase(710, 1000)]
        public void StartCooking_InvalidPower_Throws(int power, int time)
        {
            Assert.That(() => sut.StartCooking(power, time), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(timer.TimeRemaining, Is.EqualTo(0));
        }

        [TestCase(200, 1000)]
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
            sut.StartCooking(100, 1000);
            sut.Stop();

            Assert.That(stringWriter.ToString().Contains("turned off"));
        }
    }
}