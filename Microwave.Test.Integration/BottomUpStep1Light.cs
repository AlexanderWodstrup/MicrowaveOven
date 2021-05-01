using Microwave.Classes.Boundary;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class BottomUpStep1Light
    {
        private Light sut;
        private IOutput fakeOutput;
        [SetUp]
        public void Setup()
        {
            fakeOutput = Substitute.For<IOutput>();
            sut = new Light(fakeOutput);
        }

        [Test]
        public void turnOnLightWorksCorrectly()
        {
            sut.
            sut.TurnOn();
        }
    }
}