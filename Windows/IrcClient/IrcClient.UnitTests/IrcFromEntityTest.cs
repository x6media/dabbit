using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dabbit.Base;
using Dabbit.Irc;
using Dabbit.Exceptions;

namespace Dabbit.UnitTests
{
    [TestClass]
    public class IrcFromEntityTest
    {
        [TestMethod]
        public void FromEntityUserTest()
        {
            IrcFromEntity user = new IrcUserEntity("dab", "dabitp", "David Barajas", "gmail.com");
            Assert.AreEqual(typeof(IrcUserEntity), user.FromType);
            Assert.AreEqual("dab", user.Display);
            Assert.AreEqual("dab!dabitp@gmail.com", ((IrcUserEntity)user).DisplayFull);
        }

        [TestMethod]
        [ExpectedException(typeof(UserEntityNoHostException))]
        public void FromEntityUserNoHostTest()
        {
            IrcFromEntity user = new IrcUserEntity("dab", "dabitp", "David Barajas");
            string temp = ((IrcUserEntity)user).DisplayFull;
        }
    }
}
