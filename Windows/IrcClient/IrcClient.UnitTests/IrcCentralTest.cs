using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dabbit.Base;
using Dabbit.Irc;
using Dabbit.Network;
using Dabbit.Exceptions;

namespace Dabbit.UnitTests
{
    [TestClass]
    public class IrcCentralTests
    {
        [TestInitialize]
        public void Startup()
        {
            this.ctx = new IrcContext();

            IrcCentral.Instance(this.ctx);
        }

        [TestMethod]
        public void TestSingleton()
        {
            IrcCentral.Instance(this.ctx);
            Assert.AreEqual(0, (IrcCentral.Instance().Networks.Count), "Network Count should be 0");
        }

        [TestMethod]
        public void TestAddNetwork()
        {
            IrcCentral.Instance().AddNetwork("dab-irc", new IrcNetwork("dab-irc", new IrcConnection("host", 6667, false)));
            Assert.AreEqual(1, (IrcCentral.Instance().Networks.Count), "Network Count should be 1");
        }

        [TestMethod]
        public void TestAddNetworkAgain()
        {
            IrcCentral.Instance().AddNetwork("dab-irc", new IrcNetwork("dab-irc", new IrcConnection("host", 6667, false)));
            Assert.AreEqual(1, (IrcCentral.Instance().Networks.Count), "Network Count should be 1");

            IrcCentral.Instance().AddNetwork("dab-irc2", new IrcNetwork("dab-irc", new IrcConnection("host", 6667, false)));
            Assert.AreEqual(2, (IrcCentral.Instance().Networks.Count), "Network Count should be 2");
        }

        [TestMethod]
        public void TestDelNetwork()
        {
            IrcCentral.Instance().AddNetwork("dab-irc", new IrcNetwork("dab-irc", new IrcConnection("host", 6667, false)));
            Assert.AreEqual(1, (IrcCentral.Instance().Networks.Count), "Network Count should be 1");

            IrcCentral.Instance().DelNetwork("dab-irc");
            Assert.AreEqual(0, (IrcCentral.Instance().Networks.Count), "Network Count should be 0");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotExistsException))]
        public void TestDelNetworkNotExist()
        {
            IrcCentral.Instance().DelNetwork("dab-irc2");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyAlreadyExistsException))]
        public void TestAddNetworkDuplicate()
        {
            IrcCentral.Instance().AddNetwork("dab-irc", new IrcNetwork("dab-irc", new IrcConnection("host", 6667, false)));
            Assert.AreEqual(1, (IrcCentral.Instance().Networks.Count), "Network Count should be 1");
            IrcCentral.Instance().AddNetwork("dab-irc", new IrcNetwork("dab-irc", new IrcConnection("host", 6667, false)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddNetworkNullKey()
        {
            IrcCentral.Instance().AddNetwork(null, new IrcNetwork("network name", new IrcConnection("host", 6667, false)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddNetworkNullValue()
        {
            IrcCentral.Instance().AddNetwork("name", null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddSettingkNullKey()
        {
            IrcCentral.Instance().SetSetting(null, "Value");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddSettingNullValue()
        {
            IrcCentral.Instance().SetSetting("key", null);
        }

        [TestMethod]
        public void TestAddSetting()
        {
            IrcCentral.Instance().SetSetting("key", "Value");
            Assert.IsTrue(IrcCentral.Instance().Settings.ContainsKey("key"), "Key was never set");
            Assert.AreEqual("Value", IrcCentral.Instance().Settings["key"], "Value was not preserved");
            Assert.IsFalse(IrcCentral.Instance().Settings.ContainsKey("Key2"), "Key was not preserved");
        }

        [TestMethod]
        public void TestAddSettingAgain()
        {
            IrcCentral.Instance().SetSetting("key", "Value");
            Assert.IsTrue(IrcCentral.Instance().Settings.ContainsKey("key"));
            Assert.AreEqual("Value", IrcCentral.Instance().Settings["key"]);

            IrcCentral.Instance().SetSetting("Key2", "value");
            Assert.IsTrue(IrcCentral.Instance().Settings.ContainsKey("key"));
            Assert.AreEqual("Value", IrcCentral.Instance().Settings["key"]);
            Assert.IsTrue(IrcCentral.Instance().Settings.ContainsKey("Key2"));
            Assert.AreEqual("value", IrcCentral.Instance().Settings["Key2"]);
            Assert.IsFalse(IrcCentral.Instance().Settings.ContainsKey("key2"));
        }

        [TestMethod]
        public void TestDelSetting()
        {
            IrcCentral.Instance().SetSetting("dab-irc", "value");
            Assert.AreEqual(1, (IrcCentral.Instance().Settings.Count), "Settings Count should be 1");

            IrcCentral.Instance().DelSetting("dab-irc");
            Assert.AreEqual(0, (IrcCentral.Instance().Settings.Count), "Settings Count should be 0");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotExistsException))]
        public void TestDelSettingNotExist()
        {
            IrcCentral.Instance().DelNetwork("dab-irc2");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyAlreadyExistsException))]
        public void TestAddSettingDuplicate()
        {
            IrcCentral.Instance().AddNetwork("dab-irc", new IrcNetwork("dab-irc", new IrcConnection("host", 6667, false)));
            Assert.AreEqual(1, (IrcCentral.Instance().Networks.Count), "Network Count should be 1");
            IrcCentral.Instance().AddNetwork("dab-irc", new IrcNetwork("dab-irc", new IrcConnection("host", 6667, false)));
        }
        
        [TestCleanup]
        public void CleanUp()
        {
            IrcCentral.Instance().ResetState(this.ctx);
        }

        IIrcContext ctx = null;
    }
}
