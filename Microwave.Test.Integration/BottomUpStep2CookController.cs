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
        private CookController sut;
        private Light light;
        private Display display;
        private PowerTube powerTube;
        private Output output;
        private Timer timer;
        [SetUp]
        public void Setup()
        {
            output = new Output();
            timer = new Timer();
            light = new Light(output);
            display = new Display(output);
            powerTube = new PowerTube(output);

            sut = new CookController(timer,display,powerTube);
        }
    }
}