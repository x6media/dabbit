﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using dabbit.Base;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace dabbit.UnitTests
{
    [TestClass]
    public class ServerTest
    {
        [TestMethod]
        public void TestTestSocket()
        {
            string input = ":hyperion.gamergalaxy.net NOTICE AUTH :*** Looking up your hostname...\r\n";

            MemoryStream read = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(input));
            MemoryStream write = new MemoryStream();

            StringTestSocket sts = new StringTestSocket(read, write);

            string response = sts.Reader.ReadLine();
            Assert.AreEqual(input, response + "\r\n", "Lines did not match up");

            sts.Writer.WriteLine("OMG LIKE HELLO!");
            sts.Writer.Flush();
            write.Position = 0;

            StreamReader str = new StreamReader(write);

            string result = str.ReadToEnd();

            Assert.AreEqual("OMG LIKE HELLO!\r\n", result, "Write lines did not match up");
        }

        AutoResetEvent serverParseTest = new AutoResetEvent(false);

        [TestMethod]
        public void ServerParseTest()
        {
            TestContext ctx = new TestContext();
            string input = @":hyperion.gamergalaxy.net 001 lkjfsdlkf :Welcome to the Gamer Galaxy IRC IRC Network lkjfsdlkf!dabitp@76.178.154.95
:hyperion.gamergalaxy.net 002 lkjfsdlkf :Your host is hyperion.gamergalaxy.net, running version Unreal3.2.8.1
:hyperion.gamergalaxy.net 003 lkjfsdlkf :This server was created Sat Mar 2 2013 at 02:30:18 MSK
:hyperion.gamergalaxy.net 004 lkjfsdlkf hyperion.gamergalaxy.net Unreal3.2.8.1 iowghraAsORTVSxNCWqBzvdHtGp lvhopsmntikrRcaqOALQbSeIKVfMCuzNTGj
:hyperion.gamergalaxy.net 005 lkjfsdlkf CMDS=KNOCK,MAP,DCCALLOW,USERIP UHNAMES NAMESX SAFELIST HCN MAXCHANNELS=60 CHANLIMIT=#:60 MAXLIST=b:60,e:60,I:60 NICKLEN=30 CHANNELLEN=32 TOPICLEN=307 KICKLEN=307 AWAYLEN=307 :are supported by this server
:hyperion.gamergalaxy.net 005 lkjfsdlkf MAXTARGETS=20 WALLCHOPS WATCH=128 WATCHOPTS=A SILENCE=15 MODES=12 CHANTYPES=# PREFIX=(qaohv)~&@%+ CHANMODES=beI,kfL,lj,psmntirRcOAQKVCuzNSMTG NETWORK=Gamer-Galaxy-IRC CASEMAPPING=ascii EXTBAN=~,cqnrT ELIST=MNUCT :are supported by this server
:hyperion.gamergalaxy.net 005 lkjfsdlkf STATUSMSG=~&@%+ EXCEPTS INVEX :are supported by this server
:dab!dabitp@dab.biz PRIVMSG #dab :Hello World
:dab!dabitp@dab.biz PRIVMSG +#dab :Hello World
:dab!dabitp@dab.biz PRIVMSG dab :Hello World
:dab!dabitp@dab.biz NOTICE #dab :Hello World
:dab!dabitp@dab.biz NOTICE +#dab :Hello World
:dab!dabitp@dab.biz NOTICE dab :Hello World
:dab!dabitp@dab.biz NOTICE #dab :" + "\001ACTION Hello World\001" + @"
:dab!dabitp@dab.biz PRIVMSG #dab :" + "\001ACTION Hello World\001" + @"
:dab!dabitp@dab.biz NOTICE +#dab :" + "\001ACTION Hello World\001" + @"
:dab!dabitp@dab.biz PRIVMSG +#dab :" + "\001ACTION Hello World\001" + @"
:dab!dabitp@dab.biz NOTICE dab :" + "\001ACTION Hello World\001" + @"
:dab!dabitp@dab.biz PRIVMSG dab :" + "\001ACTION Hello World\001" + @"
:lkjfsdlkf!ident@host JOIN :#dab
:hyperion.gamergalaxy.net 332 ohno #dab :Welcome to dab's personal channel!!
:hyperion.gamergalaxy.net 333 ohno #dab dab 1342941141
:hyperion.gamergalaxy.net 353 ohno = #dab :ohno!ident@host.com Jiggler!ident@host.com %DeBot!ident@host.com Guest41609!ident@host.com @synapse!ident@host.com &@DoBot!ident@host.com josh!ident@host.com &@FoxTrot!ident@host.com ~@dab!ident@host.com dab-!ident@host.com Redirect_Left!ident@host.com
:hyperion.gamergalaxy.net 366 ohno #dab :End of /NAMES list.
:hyperion.gamergalaxy.net 324 dabbbb #dab +r
:john!hello@thishost.com JOIN :#dab
:hyperion.gamergalaxy.net 999 lkjfsdlkf :END OF TEST DATA!
";
            MemoryStream read = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(input));
            MemoryStream write = new MemoryStream();

            bool onChannelMessage = false;
            bool onChannelMessageNotice = false;
            bool onChannelAction = false;
            bool onChannelActionNotice = false;

            bool onChannelMessageWall = false;
            bool onChannelMessageNoticeWall = false;
            bool onChannelActionWall = false;
            bool onChannelActionNoticeWall = false;

            bool onQueryMessage = false;
            bool onQueryMessageNotice = false;
            bool onQueryAction = false;
            bool onQueryActionNotice = false;

            bool onJoin = false;
            bool onNewChannel = false;

            // Test properties
            ctx.Reader = read;
            ctx.Writer = write;

            Thread thrd;
            ISocketWrapper sock = ctx.CreateSocket("hi", 6667, false);

            Connection con = ctx.CreateConnection(ConnectionType.Direct, sock);

            ctx.Servers.Add(new Server(ctx, new User() { Nick = "dab", Ident = "hi", Name = "omg"}, con));
            ctx.Servers[0].OnRawMessage += delegate(object sender, Message e)
            {
                if (e.Command == "999")
                {
                    serverParseTest.Set();
                }
            };

            ctx.Servers[0].OnChannelMessage += delegate(object sender, PrivmsgMessage e)
            {
                if (e.To.Parts[0] != "#dab")
                    return;

                if (e.Wall == "+")
                {
                    onChannelMessageWall = true;
                }
                else
                {
                    onChannelMessage = true;
                }
            };

            ctx.Servers[0].OnChannelMessageNotice += delegate(object sender, PrivmsgMessage e)
            {
                if (e.To.Parts[0] != "#dab")
                    return;

                if (e.Wall == "+")
                {
                    onChannelMessageNoticeWall = true;
                }
                else
                {
                    onChannelMessageNotice = true;
                }
            };

            ctx.Servers[0].OnChannelAction += delegate(object sender, PrivmsgMessage e)
            {
                if (e.To.Parts[0] != "#dab")
                    return;

                if (e.Wall == "+")
                {
                    onChannelActionWall = true;
                }
                else
                {
                    onChannelAction = true;
                }
            };

            ctx.Servers[0].OnChannelActionNotice += delegate(object sender, PrivmsgMessage e)
            {
                if (e.To.Parts[0] != "#dab")
                    return;

                if (e.Wall == "+")
                {
                    onChannelActionNoticeWall = true;
                }
                else
                {
                    onChannelActionNotice = true;
                }
            };

            ctx.Servers[0].OnQueryMessage += delegate(object sender, PrivmsgMessage e)
            {
                if (e.To.Parts[0] != "dab")
                    return;

                onQueryMessage = true;
            };

            ctx.Servers[0].OnQueryMessageNotice += delegate(object sender, PrivmsgMessage e)
            {
                if (e.To.Parts[0] != "dab")
                    return;

                onQueryMessageNotice = true;
            };

            ctx.Servers[0].OnQueryAction += delegate(object sender, PrivmsgMessage e)
            {
                if (e.To.Parts[0] != "dab")
                    return;

                onQueryAction = true;
            };

            ctx.Servers[0].OnQueryActionNotice += delegate(object sender, PrivmsgMessage e)
            {
                if (e.To.Parts[0] != "dab")
                    return;

                onQueryActionNotice = true;
            };

            ctx.Servers[0].OnJoin += delegate(object sender, JoinMessage e)
            {
                onJoin = true;
            };

            ctx.Servers[0].OnNewChannelJoin += delegate(object sender, JoinMessage e)
            {
                onNewChannel = true;
            };

            thrd = new Thread(ctx.Servers[0].Connection.ReadAsync);

            thrd.Start();


            serverParseTest.WaitOne();
            Thread.Sleep(900);

            Assert.AreEqual("lkjfsdlkf", ctx.Servers[0].Me.Nick, "Nicknames are not equal");
            Assert.AreEqual("true", ctx.Servers[0].Attributes["NAMESX"], "Namesx not detected");

            write.Position = 0;
            StreamReader resReader = new StreamReader(write);

            Assert.AreEqual("CAP LS", resReader.ReadLine(), "First command was not CAP LS");
            Assert.AreEqual("NICK dab", resReader.ReadLine(), "NICK line not given, when assumed");
            Assert.AreEqual("USER hi * * :omg", resReader.ReadLine(), "Unexpected USER");

            Assert.IsTrue(onChannelMessage, "Did not parse Channel message correctly");
            Assert.IsTrue(onChannelMessageNotice, "Did not parse Chan Msg Notice correctly");
            Assert.IsTrue(onChannelAction, "Did not parse Action Chan correctly");
            Assert.IsTrue(onChannelActionNotice, "Did not parse Action Notice correctly");

            Assert.IsTrue(onChannelMessageWall, "Did not parse Channel message wall correctly");
            Assert.IsTrue(onChannelMessageNoticeWall, "Did not parse Chan Msg Notice wall correctly");
            Assert.IsTrue(onChannelActionWall, "Did not parse Action Chan wall correctly");
            Assert.IsTrue(onChannelActionNoticeWall, "Did not parse Action Notice Wall correctly");

            Assert.IsTrue(onQueryMessage, "Did not parse query message correctly");
            Assert.IsTrue(onQueryMessageNotice, "Did not parse query Msg Notice correctly");
            Assert.IsTrue(onQueryAction, "Did not parse Action query correctly");
            Assert.IsTrue(onQueryActionNotice, "Did not parse Action Notice query correctly");

            Assert.IsTrue(onJoin, "OnJoin callback was not fired");
            Assert.IsTrue(onNewChannel, "OnNewChannelJoin was not called back");

            Channel tmp = ctx.Servers[0].Channels["#dab"];

            Assert.AreEqual("dab", tmp.Users[0].Nick, "Nicks in wrong order");
            Assert.AreEqual("DoBot", tmp.Users[1].Nick, "Nicks in wrong order");
            Assert.AreEqual("FoxTrot", tmp.Users[2].Nick, "Nicks in wrong order");
            Assert.AreEqual("synapse", tmp.Users[3].Nick, "Nicks in wrong order");
            Assert.AreEqual("DeBot", tmp.Users[4].Nick, "Nicks in wrong order");
            Assert.AreEqual("dab-", tmp.Users[5].Nick, "Nicks in wrong order");
            Assert.AreEqual("Guest41609", tmp.Users[6].Nick, "Nicks in wrong order");
            Assert.AreEqual("Jiggler", tmp.Users[7].Nick, "Nicks in wrong order");
            Assert.AreEqual("john", tmp.Users[8].Nick, "Nicks in wrong order");
            Assert.AreEqual("josh", tmp.Users[9].Nick, "Nicks in wrong order");
            Assert.AreEqual("ohno", tmp.Users[10].Nick, "Nicks in wrong order");
            Assert.AreEqual("Redirect_Left", tmp.Users[11].Nick, "Nicks in wrong order");

        }
    }

    public class TestContext : IContext
    {
        public TestContext()
        {
            this.Servers = new List<Server>();
            this.Settings = new Dictionary<string, string>();
        }

        public Connection CreateConnection(ConnectionType connectionType, ISocketWrapper socket)
        {
            return new DirectConnection(this, socket);
        }

        public List<Server> Servers { get; set; }
        public Dictionary<string, string> Settings { get; set; }
        
        public MemoryStream Reader { get; set; }
        public MemoryStream Writer { get; set; }

        /// <summary>
        /// Create an empty blank TCP socket.
        /// </summary>
        /// <returns>Blank tcp socket capable of returning read/write streams and Async Connect.</returns>
        public ISocketWrapper CreateSocket(string host, int port, bool secure)
        {
            return new StringTestSocket(this.Reader, this.Writer);
        }
    }

    public class DirectConnection : Connection
    {
        public DirectConnection(IContext ctx, ISocketWrapper sckt)
            : base(ctx, sckt)
        {
        }

        public new void Write(string message)
        {
            Console.WriteLine(message);
            base.Write(message);
        }
    }

    public class StringTestSocket : ISocketWrapper
    {
        // Request these attributes in the Constructor
        public string Host { get { return "127.0.0.1"; } }
        public int Port { get { return 6667; } }
        public bool Secure { get { return true; } }

        public bool Connected { get { return connected; } }
        private bool connected = false;

        public StringTestSocket(MemoryStream read, MemoryStream write)
        {
            this.streamRead = new StreamReader(read);
            this.streamWriter = new StreamWriter(write);
        }
        public Task ConnectAsync()
        {
            this.connected = true;
            return new Task(new Action(nope));
        }

        public void nope()
        { }

        public void Disconnect()
        {
        }

        public StreamReader Reader { get { return this.streamRead; } }

        public StreamWriter Writer { get { return this.streamWriter; } }

        private StreamReader streamRead;
        private StreamWriter streamWriter;
    }
}
