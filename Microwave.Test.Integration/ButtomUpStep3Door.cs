using System.IO;
using Microwave.Classes.Boundary;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class ButtomUpStep3Door
    {
        private Door door;

        [SetUp]
        public void Setup()
        {
            door = new Door();
        }

        [Test]

        public void doorIsClosedCorrectly()
        {
            door.Close();

        }

    }
}
