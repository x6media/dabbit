using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dabbit.Base;

namespace Dabbit.UnitTests
{
    /// <summary>
    /// Test class to test the abstraction class
    /// </summary>
    class EntityTest : IrcEntity
    {
        public EntityTest(string display)
            : base(display)
        {
        }
    }

    [TestClass]
    public class IrcEntityTests
    {
        [TestMethod]
        public void IrcEntityConstructor()
        {
            IrcEntity ircEntity = new EntityTest("Display");
            Assert.AreEqual("Display", ircEntity.Display);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IrcEntityInvalidConstructor()
        {
            new EntityTest(null);
        }
    }
}
