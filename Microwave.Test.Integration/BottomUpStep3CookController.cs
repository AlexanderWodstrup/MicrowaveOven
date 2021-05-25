using System;
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
        private IUserInterface ui;
        private CookController sut;
        private IDisplay display;
        private IPowerTube powerTube;
        private IOutput output;
        private ITimer timer;
        
        [SetUp]
        public void Setup()
        {
            ui = Substitute.For<IUserInterface>();
            output = new Output();
            timer = new Timer();
            display = new Display(output);
            powerTube = new PowerTube(output);


            sut = new CookController(timer,display,powerTube,ui);
            stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
        }


        [TestCase(-1)]
        [TestCase(10)]
        [TestCase(0)]

        public void StartCooking_TimerStart_TimeRemainingCorrect(int time)
        {

            sut.StartCooking(50, time);

            Assert.AreEqual(timer.TimeRemaining, time*1000);

        }

        [TestCase(60,60000,0)]
        [TestCase(60, 55000, 5100)]
        [TestCase(60, 49000, 11100)]
        public void StartCooking_ValidParameters_TimerStarted(int time, int timeRemaining, int sleep)
        {
            sut.StartCooking(50, time);
            Thread.Sleep(sleep);
            Assert.AreEqual(timer.TimeRemaining, timeRemaining);
            
        }

        [Test]
        public void StartCooking_ValidParameters_PowerTubeStarted()
        {
            sut.StartCooking(50, 60);

            Assert.That(stringWriter.ToString().Contains("works") && stringWriter.ToString().Contains("50"));
        }

        [TestCase(120, 5100, "1:55")]
        [TestCase(120, 1100, "1:59")]
        [TestCase(120, 11100, "1:49")]
        [TestCase(5, 5000, "0:01")]
        [TestCase(5, 6000, "turned off")]
        public void Cooking_TimerTick_DisplayCalled(int time, int sleep, string message)
        {
            sut.StartCooking(50, time);

            Thread.Sleep(sleep);

            Assert.That(stringWriter.ToString().Contains(message));
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

            ui.Received(1).CookingIsDone();
        }

        [Test]
        public void Cooking_Stop_PowerTubeOff()
        {
            sut.StartCooking(50, 60);
            sut.Stop();
            Assert.That(stringWriter.ToString().Contains("turned off"));
        }
        [Test]
        public void Cooking_Stop_AndTimer_PowerTubeOff()
        {
            sut.StartCooking(50, 60);
            Thread.Sleep(5000);
            sut.Stop();
            Assert.That(stringWriter.ToString().Contains("turned off"));
        }
        [Test]
        public void Cooking_Stop_AndTimer_PowerTubeOff2()
        {
            sut.StartCooking(50, 60);
            
            sut.Stop();
            Thread.Sleep(5000);
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