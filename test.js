var System = new require("all")("System");
var Console = System.Console;
var dabbit = new require("all")("dabbit");

var Assert = require('assert');

var writtenLines = [];

//console.log("DeBot".localeCompare("dab-"));
console.log("DeBot".CompareTo("dab-"));


function nothingWriter ()
{
    this.write = function (data) 
    { 
        if (!String.IsNullOrEmpty(data))
        {
            Console.WriteLine(data); 
            writtenLines.push(data); 
        }

    }
}

function StringTestSocket(read, write) {
    // Indicates object inheritance.
    dabbit.Base.ISocketWrapper.call(this);
    
    var rdCb = undefined;
    var connectedState = false;
    this.__defineGetter__("Host", function() {
        return "irc.gamergalaxy.net";
    });

    this.__defineGetter__("Port", function() {
        return 6697;
    });

    this.__defineGetter__("Secure", function() {
        return true;
    });

    this.__defineGetter__("Connected", function() {
        return (connectedState);
    });

    var self = this;

    var input = ":hyperion.gamergalaxy.net 001 lkjfsdlkf :Welcome to the Gamer Galaxy IRC IRC Network lkjfsdlkf!dabitp@76.178.154.95" + "\r\n" +
        ":hyperion.gamergalaxy.net 002 lkjfsdlkf :Your host is hyperion.gamergalaxy.net, running version Unreal3.2.8.1" + "\r\n" +
        ":hyperion.gamergalaxy.net 003 lkjfsdlkf :This server was created Sat Mar 2 2013 at 02:30:18 MSK" + "\r\n" +
        ":hyperion.gamergalaxy.net 004 lkjfsdlkf hyperion.gamergalaxy.net Unreal3.2.8.1 iowghraAsORTVSxNCWqBzvdHtGp lvhopsmntikrRcaqOALQbSeIKVfMCuzNTGj" + "\r\n" +
        ":hyperion.gamergalaxy.net 005 lkjfsdlkf CMDS=KNOCK,MAP,DCCALLOW,USERIP UHNAMES NAMESX SAFELIST HCN MAXCHANNELS=60 CHANLIMIT=#:60 MAXLIST=b:60,e:60,I:60 NICKLEN=30 CHANNELLEN=32 TOPICLEN=307 KICKLEN=307 AWAYLEN=307 :are supported by this server" + "\r\n" +
        ":hyperion.gamergalaxy.net 005 lkjfsdlkf MAXTARGETS=20 WALLCHOPS WATCH=128 WATCHOPTS=A SILENCE=15 MODES=12 CHANTYPES=# PREFIX=(qaohv)~&@%+ CHANMODES=beI,kfL,lj,psmntirRcOAQKVCuzNSMTG NETWORK=Gamer-Galaxy-IRC CASEMAPPING=ascii EXTBAN=~,cqnrT ELIST=MNUCT :are supported by this server" + "\r\n" +
        ":hyperion.gamergalaxy.net 005 lkjfsdlkf STATUSMSG=~&@%+ EXCEPTS INVEX :are supported by this server" + "\r\n" +
        ":dab!dabitp@dab.biz PRIVMSG #DAB :Hello World" + "\r\n" +
        ":dab!dabitp@dab.biz PRIVMSG +#DAB :Hello World" + "\r\n" +
        ":dab!dabitp@dab.biz PRIVMSG dab :Hello World" + "\r\n" +
        ":dab!dabitp@dab.biz NOTICE #DAB :Hello World" + "\r\n" +
        ":dab!dabitp@dab.biz NOTICE +#DAB :Hello World" + "\r\n" +
        ":dab!dabitp@dab.biz NOTICE dab :Hello World" + "\r\n" +
        ":dab!dabitp@dab.biz NOTICE #DAB :" + "\001ACTION Hello World\001" + "\r\n" +
        ":dab!dabitp@dab.biz PRIVMSG #DAB :" + "\001ACTION Hello World\001" + "\r\n" +
        ":dab!dabitp@dab.biz NOTICE +#DAB :" + "\001ACTION Hello World\001" + "\r\n" +
        ":dab!dabitp@dab.biz PRIVMSG +#DAB :" + "\001ACTION Hello World\001" + "\r\n" +
        ":dab!dabitp@dab.biz NOTICE dab :" + "\001ACTION Hello World\001" + "\r\n" +
        ":dab!dabitp@dab.biz PRIVMSG dab :" + "\001ACTION Hello World\001" + "\r\n" +
        ":lkjfsdlkf!ident@host JOIN :#DAB" + "\r\n" +
        ":hyperion.gamergalaxy.net 332 lkjfsdlkf #DAB :Welcome to dab's personal channel!!" + "\r\n" +
        ":hyperion.gamergalaxy.net 333 lkjfsdlkf #DAB dab 1342941141" + "\r\n" +
        ":hyperion.gamergalaxy.net 353 lkjfsdlkf = #DAB :lkjfsdlkf!ident@host.com Jiggler!ident@host.com %+DeBot!ident@host.com Guest41609!ident@host.com @synapse!ident@host.com &@DoBot!ident@host.com josh!ident@host.com &@FoxTrot!ident@host.com ~@dab!ident@host.com %dab-!ident@host.com Redirect_Left!ident@host.com" + "\r\n" +
        ":hyperion.gamergalaxy.net 366 lkjfsdlkf #DAB :End of /NAMES list." + "\r\n" +
        ":unit@test.framework.net 998 lkjfsdlkf :CHECK NAMES" + "\r\n" +
        ":hyperion.gamergalaxy.net 324 dabbbb #DAB +rknt hi" + "\r\n" +
        ":unit@test.framework.net 996 lkjfsdlkf :CHECK MODES" + "\r\n" +
        ":john!hello@thishost.com JOIN :#DAB" + "\r\n" +
        ":john2!hello@thishost.com JOIN :#DAB" + "\r\n" +
        ":john2quit!hello@thishost.com JOIN :#DAB" + "\r\n" +
        ":johnnick1!hello@thishost.com JOIN :#DAB" + "\r\n" +
        ":kickme!hello@thishost.com JOIN :#DAB" + "\r\n" +
        ":hyperion.gamergalaxy.net MODE #DAB +bbbI-b dab!*@* other!*@* third!*@* other!*@* dab!*@*" + "\r\n" +
        ":unit@test.framework.net 997 lkjfsdlkf :CHECK BANS" + "\r\n" +
        ":dab!dabitp@dab.biz MODE #DAB +a john" + "\r\n" +
        ":lkjfsdlkf!dabitp@dab.biz MODE lkjfsdlkf +az" + "\r\n" +
        ":lkjfsdlkf!dabitp@dab.biz MODE lkjfsdlkf -z" + "\r\n" +
        ":john2!hello@thishost.com PART #DAB" + "\r\n" +
        ":john2quit!hello@thishost.com QUIT :Bye bye!" + "\r\n" +
        ":johnnick1!hello@thishost.com NICK zzzbad" + "\r\n" +
        ":dab!dabitp@dab.biz KICK #DAB kickme :Reason ok?" + "\r\n" +
        ":hyperion.gamergalaxy.net 999 lkjfsdlkf :END OF TEST DATA!" + "\r\n" +
        "";

    this.ConnectAsync = function(rawData) {
        rdCb = rawData || function() { };
        console.log("TICK!");
        connectedState = true;
        setTimeout(function() { onData(input) }, 1000);
    };

    this.Disconnect = function() {
        connectedState = false;
    }

    var backlog = "";
    // http://stackoverflow.com/a/10012306/486058
    var onData = function(data) {
        backlog = data;
        var n = backlog.indexOf('\n');

        // got a \n? emit one or more 'line' events
        while (~n) {
            //stream.emit('line', backlog.substring(0, n))
            if (backlog[n-1] == '\r') {
                rdCb(backlog.substring(0, n-1).trim());
            } else {
                rdCb(backlog.substring(0, n));
            }

            backlog = backlog.substring(n + 1);
            n = backlog.indexOf('\n');
        }

    }

    this.__defineGetter__("Reader", function() {
        throw new System.MustImplementException("Reader");
    });

    this.__defineGetter__("Writer", function() {
        return new nothingWriter();
    });
}
System.Javascript.Inherit(dabbit.Base.ISocketWrapper, StringTestSocket);


function TestContext() {
    // Indicates object inheritance.
    dabbit.Base.IContext.call(this);
    
    /// <summary>
    /// Create a connection object given ConnectionType and ISocketWrapper
    /// </summary>
    this.CreateConnection = function(connectionType, socket) {
        var connection = new dabbit.Base.Connection(this, socket);
                
        return connection;
    }

    this.AddServer = function(me, con) {
        var svr = new dabbit.Base.Server(this, me, con);
        this.Server = svr;
    }
    
    this.Server =  {};

    this.Settings = {};
    
    /// <summary>
    /// Creates a blank TCP Socket for use in a connection
    /// </summary>
    /// <param name="host">The host you want to connect to.</param>
    /// <param name="port">port</param>
    /// <param name="secure">bool is secure</param>
    /// <returns>A socket wrapper for a connection. This is platform dependant. This will not be in the same namespace</returns>
    this.CreateSocket = function(host, port, secure) {
        return new StringTestSocket(host, port, secure);
    }

    /// <summary>
    /// Create a channel object
    /// <param name="svr">The Server object for applying this to. Used for passing into the Channel base class</param>
    /// <returns>An object that either is, or inherits from Channel</returns>
    this.CreateChannel = function(svr) {
        return new dabbit.Base.Channel(svr);
    }

    /// <summary>
    /// Create a channel object
    /// <param name="source">(OPTIONAL) The SourceEntity object we want to create the user from. If no SourceEntity is passed, we pass an empty user</param>
    /// <returns>An object that either is, or inherits from User</returns>
    this.CreateUser = function(source) {
        return new dabbit.Base.User(source);
    }
}
System.Javascript.Inherit(dabbit.Base.IContext, TestContext);
function censor(censor) {
  var i = 0;

  return function(key, value) {
    if(i !== 0 && typeof(censor) === 'object' && typeof(value) == 'object' && censor == value) 
      return '[Circular]'; 

    //if(i >= 29) // seems to be a harded maximum of 30 serialized objects?
    //  return '[Unknown]';

    ++i; // so we know we aren't using the original object anymore

    return value;  
  }
}


var onChannelMessage = false;
var onChannelMessageNotice = false;
var onChannelAction = false;
var onChannelActionNotice = false;

var onChannelMessageWall = false;
var onChannelMessageNoticeWall = false;
var onChannelActionWall = false;
var onChannelActionNoticeWall = false;

var onQueryMessage = false;
var onQueryMessageNotice = false;
var onQueryAction = false;
var onQueryActionNotice = false;

var onJoin = false;
var onNewChannel = false;

var onQuitTest = false;
var onNumeric333 = false;


var ctx = new TestContext();

var sock = ctx.CreateSocket("hi", 6667, false);

var con = ctx.CreateConnection(dabbit.Base.ConnectionType.Direct, sock);

var me = new dabbit.Base.User();
me.Nick = "lkjfsdlkf";
me.Ident = "hi";
me.Name = "omg";

ctx.Server = new dabbit.Base.Server(ctx, me, con);

ctx.Server.Events.on('OnRawMessage', function(svr, e) {
    //console.log(e);
    if ("999" == e.Command) {
        Assert.equal("lkjfsdlkf", ctx.Server.Me.Nick, "Nicknames are not equal");
        Assert.equal("true", ctx.Server.Attributes["NAMESX"], "Namesx not detected");
        
        var readI = 0;

        Assert.equal("CAP LS", writtenLines[readI++].trim(), "First command was not CAP LS");
        Assert.equal("NICK lkjfsdlkf", writtenLines[readI++].trim(), "NICK line not given, when assumed");
        Assert.equal("USER hi * * :omg", writtenLines[readI++].trim(), "Unexpected USER");

        Assert(onChannelMessage, "Did not parse Channel message correctly");
        Assert(onChannelMessageNotice, "Did not parse Chan Msg Notice correctly");
        Assert(onChannelAction, "Did not parse Action Chan correctly");
        Assert(onChannelActionNotice, "Did not parse Action Notice correctly");

        Assert(onChannelMessageWall, "Did not parse Channel message wall correctly");
        Assert(onChannelMessageNoticeWall, "Did not parse Chan Msg Notice wall correctly");
        Assert(onChannelActionWall, "Did not parse Action Chan wall correctly");
        Assert(onChannelActionNoticeWall, "Did not parse Action Notice Wall correctly");

        Assert(onQueryMessage, "Did not parse query message correctly");
        Assert(onQueryMessageNotice, "Did not parse query Msg Notice correctly");
        Assert(onQueryAction, "Did not parse Action query correctly");
        Assert(onQueryActionNotice, "Did not parse Action Notice query correctly");

        Assert(onJoin, "OnJoin callback was not fired");
        Assert(onNewChannel, "OnNewChannelJoin was not called back");
        Assert(onNumeric333, "Numeric 333 was not called! :(");

        Assert(onQuitTest, "Quit test did not pass. Did not quit with proper channels");

        var tmp = ctx.Server.Channels["#dab"];
        console.log(tmp.Users);
        var order = 0;
        Assert.equal(tmp.Users[order++].Nick, "dab", "dab Nicks in wrong order");
        Assert.equal("DoBot", tmp.Users[order++].Nick, "DoBot Nicks in wrong order");
        Assert.equal("FoxTrot", tmp.Users[order++].Nick, "FoxTrot Nicks in wrong order");
        Assert.equal("john", tmp.Users[order++].Nick, "john Nicks in wrong order");
        Assert.equal("synapse", tmp.Users[order++].Nick, "synapse Nicks in wrong order");
        Assert.equal("dab-", tmp.Users[order++].Nick, "dab- Nicks in wrong order");
        Assert.equal("DeBot", tmp.Users[order++].Nick, "DeBot Nicks in wrong order");
        Assert.equal("Guest41609", tmp.Users[order++].Nick, "Guest Nicks in wrong order");
        Assert.equal("Jiggler", tmp.Users[order++].Nick, "Jiggler Nicks in wrong order");
        Assert.equal("josh", tmp.Users[order++].Nick, "josh Nicks in wrong order");
        Assert.equal("lkjfsdlkf", tmp.Users[order++].Nick, "lkjfsdlkf Nicks in wrong order");
        Assert.equal("Redirect_Left", tmp.Users[order++].Nick, "RDL Nicks in wrong order");

        Assert.equal("hi", tmp.Modes[1].Argument, "Argument key not hi");

        Assert.equal('a', ctx.Server.Me.Modes[0][0], "The Me variable should have the a mode set");

        Assert.equal(2, tmp.Modes.Where(function(p) { return p.Character == 'b'; }).length, "Only 2 +b modes should be set");
        Assert(! tmp.Modes.Where(function(p) { return p.Character == 'i'}).First(), "Channel Mode i should have been removed");
        Assert.ifError(ctx.Server.Me.Modes.Where(function(p) { return p == "z"}).First(), "User Mode z should have been removed");
        Assert.ifError(tmp.Users.Where(function(p) { return p.Nick == "john2" }).First(), "John2 should have been removed from the user list");
        Assert.ifError(tmp.Users.Where(function(p) { return p.Nick == "kickme"}).First(), "kickme should have been removed from the user list");
        Assert.ifError(tmp.Users.Where(function(p) { return p.Nick == "johnnick1"}).First(), "johnnick1 should have been removed and changed to kickme on nick");
        Assert(tmp.Users.Where(function(p) { return p.Nick == "zzzbad"}).First(), "Nickname change for zzzbad was not updated");
        console.log(JSON.stringify(ctx.Server, censor(ctx.Server)));
        console.log("Unit tests pass");
    }
});

ctx.Server.Events.on('OnChannelMessage', function(svr, e) {
    if (e.To.Parts[0] != "#DAB")
        return;

    if (e.Wall == "+")
    {
        onChannelMessageWall = true;
    }
    else
    {
        onChannelMessage = true;
    }
});

ctx.Server.Events.on('OnChannelMessageNotice', function(svr, e) {
    if (e.To.Parts[0] != "#DAB")
        return;

    if (e.Wall == "+")
    {
        onChannelMessageNoticeWall = true;
    }
    else
    {
        onChannelMessageNotice = true;
    }
});

ctx.Server.Events.on('OnChannelAction', function(svr, e) {
    if (e.To.Parts[0] != "#DAB")
        return;

    if (e.Wall == "+")
    {
        onChannelActionWall = true;
    }
    else
    {
        onChannelAction = true;
    }
});

ctx.Server.Events.on('OnChannelActionNotice', function(svr, e) {
    if (e.To.Parts[0] != "#DAB")
        return;

    if (e.Wall == "+")
    {
        onChannelActionNoticeWall = true;
    }
    else
    {
        onChannelActionNotice = true;
    }
});

ctx.Server.Events.on('OnQueryMessage', function(svr, e) {
    if (e.To.Parts[0] != "dab")
        return;

    onQueryMessage = true;
});

ctx.Server.Events.on('OnQueryMessageNotice', function(svr, e) {
    if (e.To.Parts[0] != "dab")
        return;

    onQueryMessageNotice = true;
});

ctx.Server.Events.on('OnQueryAction', function(svr, e) {
    if (e.To.Parts[0] != "dab")
        return;

    onQueryAction = true;
});

ctx.Server.Events.on('OnQueryActionNotice', function(svr, e) {
    if (e.To.Parts[0] != "dab")
        return;

    onQueryActionNotice = true;
});

ctx.Server.Events.on('OnJoin', function(svr, e) {
    onJoin = true;
});

ctx.Server.Events.on('OnNewChannelJoin', function(svr, e) {
    onNewChannel = true;
});

ctx.Server.Events.on('OnQuit', function(svr, e) {
    if (e.Channels[0] == "#dab")
    {
        onQuitTest = true;
    }
});

ctx.Server.Events.on('333', function(svr, e) {
    onNumeric333 = true;
});
ctx.Server.PerformConnect();
//var net = require('net');
//net.createServer().listen(9991);