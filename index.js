"use strict";

var restify = require("restify");
var http = require('http');
var fs = require("fs");
var WebSocketServer = require('websocket').server;

var System = new require("all")("System");
var Console = System.Console;

var dabbit = new require("all")("dabbit");



var server = restify.createServer();
server.use(restify.bodyParser());

var ircServer = "irc.dab.biz";
var ircPort = 6667;
var ircSsl = false;


var apiCalls = [ 
	{"name": "createConnection", "func":createConnection},
	{"name": "login", "func":login},
	{"name": "register", "func":register},
	{"name": "startSession", "func":startSession},	
	{"name": "extendSession", "func":extendSession},
	{"name": "subscribePushNotification", "func":subscribePushNotification},
];
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
function createConnection(req, res, next)
{
	if (!verifySessionToken(req.params.sessionToken))
	{
		return next(new restify.errors.RequestExpiredError("Invalid session Token"));
	}

	if (!verifyNetworkObject(req.params.network))
	{
		return next(new restify.errors.RequestExpiredError("Invalid session Token"));
	}

	res.send(JSON.stringify({ "wsurl":"ws://jupiter.dabberz.com:8080/ws/7N2n9cOlk0sa8jTGf6NdpexUp6YCdzQJ"}));

	next();
}

function login(req, res, next)
{
	if (!verifySessionToken(req.params.sessionToken))
	{
		return next(new restify.errors.RequestExpiredError("Invalid session Token"));
	}

	if (!verifyUser(req.params.email, req.params.password))
	{
		return next(new restify.errors.InvalidCredentialsError("Invalid login info"));
	}

	res.send(JSON.stringify({ "code":"success", "messages":[ {"key":"login", "value":"Authentication successfull"}]}));
	next();
}

function register(req, res, next)
{
	var errorCount = 0;
	var errors = [];

	if (!verifySessionToken(req.params.sessionToken))
	{
		return next(new restify.errors.RequestExpiredError("Invalid session Token"));
	}

	if (!verifyEmailAddress(req.params.email))
	{
		errorCount++;
		array_push(errors, {"key":"email", "value":"Invalid email format" } );
	}

	if (!registerAccount(req.params))
	{
		errorCount++;
		array_push(errors, {"key":"login", "value":"This login is already in use or the email is already in use" } );
	}


	if (errorCount > 0)
	{
		res.send(JSON.stringify({ "code":"error", "messages": errors }));
	}
	else
	{
		res.send(JSON.stringify({ "code":"success", "messages":[ {"key":"login", "value":"Authentication successfull"}]}));
	}

	next();
}

function startSession(req, res, next)
{
	res.send(JSON.stringify({ "code":"success", "sessionToken": "P0b8dv3A8pcIpMOiJnQqZ2iPIZoYQMpf"}));

	next();
}

function extendSession(req, res, next)
{
	if (!verifySessionToken(req.params.sessionToken))
	{
		return next(new restify.errors.RequestExpiredError("Invalid session Token"));
	}
	res.send(JSON.stringify({ "code":"success", "sessionToken": "P0b8dv3A8pcIpMOiJnQqZ2iPIZoYQMpf"}));

	next();
}

function subscribePushNotification(req, res, next)
{
	if (!verifySessionToken(req.params.sessionToken))
	{
		return next(new restify.errors.RequestExpiredError("Invalid session Token"));
	}
	res.send(JSON.stringify({ "code":"success", "messages": [] }));

	next();
}

function verifySessionToken(sessionToken)
{

	return sessionToken != undefined && sessionToken.length > 0;
}

function verifyNetworkObject(networkObj)
{
	return networkObj != undefined && networkObj.host != undefined && networkObj.port != undefined && networkObj.ssl != undefined;
}

function verifyConnectionToken(connectionToken)
{
	return connectionToken != undefined && connectionToken.length > 0;
}


function verifyEmailAddress(email)
{
	return email != undefined && email.length > 0 && (email.match(/@/g));
}

function registerAccount(details)
{
	return details != undefined && details.login != undefined && details.email != undefined && details.password != undefined;
}


apiCalls.forEach(function(val, ind, ar) 
	{ 
		server.post("/" + val.name, val.func); 
		Console.WriteLine("Added api {0}", val.name);
	}
);

server.pre(restify.pre.userAgentConnection());

server.listen(8080);

var websocket = new WebSocketServer({httpServer:server});

websocket.on('request', function(request) {

	var connection = request.accept(null, request.origin);

	// check for valid key:
	if (!verifyConnectionToken(request.resource.split('/')[2]))
	{
		connection.sendUTF(JSON.stringify({"code":"error", "message":"Invalid connection token"}));
		connection.close();
		return;
	}


	var context = new dabbit.Node.NodeContext();
	// this.CreateSocket = function(host, port, secure) {
	// this.CreateConnection = function(connectionType, socket) {
	// this.AddServer = function(me, con) {

	
    connection.on('message', function(message) {
    	console.log(message.type);
        if (message.type === 'utf8') {
        	if (message.utf8Data[0] != "{") return connection.sendUTF(JSON.stringify({"code":"error", "message":"Invalid command", "received":message.utf8Data}));

        	try {
            var obj = JSON.parse(message.utf8Data);

        	} catch(ex) {
        		console.log(ex);
        		 return connection.sendUTF(JSON.stringify({"code":"error", "message":ex.message, "received":message.utf8Data}));
        	}

            switch(obj.command) {
            	case "sendraw":
            		var parts = (":" + context.Server.Me.Nick + "!" + context.Server.Me.Ident + "@" + context.Server.Me.Host + " " + obj.message).split(' ');
            		if (parts[0] == "PRIVMSG")
            		{
            			var msg = new dabbit.Base.Message();
            			msg.Parts = parts;
            			msg.MessageLine = msg.Parts[3].substring(1);
            			for(var i = 4; i < msg.Parts.length; i++)
            				msg.MessageLine += " " + msg.Parts[i];
            			msg.RawLine = ":" + context.Server.Me.Nick + "!" + context.Server.Me.Ident + "@" + context.Server.Me.Host + " " + obj.message;
            			msg.Timestamp = new Date();
            			msg.Command = "PRIVMSG";
            			msg.From = new dabbit.Base.SourceEntity(context.Server.Me);

            			var privmsgmsg = new dabbit.Base.Events.PrivmsgMessage(msg);
            			privmsgmsg.To = new dabbit.Base.SourceEntity([msg.Parts[2]], (context.Server.Attributes["CHANTYPES"].indexOf(msg.Parts[2][0]) != -1 ?  "Channel" : "Client" ));
            		}
            		context.Server.Connection.Write(obj.message);

            	break;
            	case "connect":
            		if (!obj.parameters.nick || !obj.parameters.ident || !obj.parameters.real) {
        				 return connection.sendUTF(JSON.stringify({"code":"error", "message":"Missing nick, ident, or real name", "received":message.utf8Data}));
            		}

            		if (context.Server.Connection && true == context.Server.Connection.Connected) {
        				 return connection.sendUTF(JSON.stringify({"code":"error", "message":"You are already connected"}));
            		}
					var me = new dabbit.Base.User();
					me.Nick = "dabbit";
					me.Ident = "dabitp";
					me.Name = "David";

					context.AddServer(me, context.CreateConnection(dabbit.Base.ConnectionType.Direct, context.CreateSocket(ircServer, ircPort, ircSsl)));

					context.Server.Events.on("OnRawMessage", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnRawMessage", "parameters" : msg })); } );
					context.Server.Events.on("OnChannelAction", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnChannelAction", "parameters" : msg })); } );
					context.Server.Events.on("OnChannelMessage", function(svr, msg) { 
						connection.sendUTF(JSON.stringify({"event": "OnChannelMessage", "parameters" : msg }));
						if (msg.MessageLine == "!listusers") {
							var users = context.Server.Channels[msg.To.Parts[0].toLowerCase()].Users;

							for(var i = 0; i < users.length; i++) {
								context.Server.Connection.Write("PRIVMSG " + msg.To.Parts[0] + " :" + users[i].Display);
							}
						}
					});

					context.Server.Events.on("OnQueryAction", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnQueryAction", "parameters" : msg })); } );
					context.Server.Events.on("OnQueryMessage", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnQueryMessage", "parameters" : msg })); } );
					context.Server.Events.on("OnChannelActionNotice", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnChannelActionNotice", "parameters" : msg })); } );
					context.Server.Events.on("OnChannelMessageNotice", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnChannelMessageNotice", "parameters" : msg })); } );
					context.Server.Events.on("OnQueryActionNotice", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnQueryActionNotice", "parameters" : msg })); } );
					context.Server.Events.on("OnQueryMessageNotice", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnQueryMessageNotice", "parameters" : msg })); } );
					context.Server.Events.on("OnError", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnError", "parameters" : msg })); } );
					context.Server.Events.on("OnNewChannelJoin", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnNewChannelJoin", "parameters" : msg })); } );
					context.Server.Events.on("OnJoin", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnJoin", "parameters" : msg })); } );
					context.Server.Events.on("OnPart", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnPart", "parameters" : msg })); } );
					context.Server.Events.on("OnCloseChannelPart", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnCloseChannelPart", "parameters" : msg })); } );
					context.Server.Events.on("OnKick", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnKick", "parameters" : msg })); } );
					context.Server.Events.on("OnNickChange", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnNickChange", "parameters" : msg })); } );
					context.Server.Events.on("OnBan", function(svr, msg) {connection.sendUTF(JSON.stringify({"event": "OnBan", "parameters" : msg })); } );
					context.Server.Events.on("OnUnban", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnUnban", "parameters" : msg })); } );
					context.Server.Events.on("OnModeChange", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnModeChange", "parameters" : msg })); } );
					context.Server.Events.on("OnTopic", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnTopic", "parameters" : msg })); } );
					context.Server.Events.on("OnAway", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnAway", "parameters" : msg })); } );
					context.Server.Events.on("OnUnAway", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnUnAway", "parameters" : msg })); } );
					context.Server.Events.on("OnInvite", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnInvite", "parameters" : msg })); } );
					context.Server.Events.on("OnWhoIs", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnWhoIs", "parameters" : msg })); } );
					context.Server.Events.on("OnConnectionEstablished", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnConnectionEstablished", "parameters" : msg })); } );
					context.Server.Events.on("OnMotd", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnMotd", "parameters" : msg })); } );
					context.Server.Events.on("OnList", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnList", "parameters" : msg })); } );
					context.Server.Events.on("OnUnhandledEvent", function(svr, msg) { connection.sendUTF(JSON.stringify({"event": "OnUnhandledEvent", "parameters" : msg })); } );
					context.Server.PerformConnect();

            	break;
            	case "msg":
            		if (!obj.parameters.message || !obj.parameters.destination) {
        				 return connection.sendUTF(JSON.stringify({"code":"error", "message":"Missing message or destination", "received":message.utf8Data}));
            		}

            		var line = "PRIVMSG " + obj.parameters.destination + " :" + obj.parameters.message;

            		var parts = (":" + context.Server.Me.Nick + "!" + context.Server.Me.Ident + "@" + context.Server.Me.Host + " " + line).split(' ');
        			var msg = new dabbit.Base.Message();
        			msg.Parts = parts;
        			msg.MessageLine = msg.Parts[3].substring(1);
        			for(var i = 4; i < msg.Parts.length; i++)
        				msg.MessageLine += " " + msg.Parts[i];
        			msg.RawLine = ":" + context.Server.Me.Nick + "!" + context.Server.Me.Ident + "@" + context.Server.Me.Host + " " + line;
        			msg.Timestamp = new Date();
        			msg.Command = "PRIVMSG";
        			msg.From = new dabbit.Base.SourceEntity(context.Server.Me);

        			var privmsgmsg = new dabbit.Base.Events.PrivmsgMessage(msg);
        			privmsgmsg.To = new dabbit.Base.SourceEntity([msg.Parts[2]], (context.Server.Attributes["CHANTYPES"].indexOf(msg.Parts[2][0]) != -1 ?  "Channel" : "Client" ));
            		context.Server.Connection.Write(line);
            	break;
            	case "join":
            		if (!obj.parameters.channel) {
        				return connection.sendUTF(JSON.stringify({"code":"error", "message":"Channel to join required!", "received":message.utf8Data}));
            		}
            		context.Server.Connection.Write("JOIN " + obj.parameters.channel + " :" + obj.parameters.key);
            	break;
            	case "part":
            		if (!obj.parameters.channel) {
        				return connection.sendUTF(JSON.stringify({"code":"error", "message":"Channel to join required!", "received":message.utf8Data}));
            		}
            		context.Server.Connection.Write("PART " + obj.parameters.channel + " :" + obj.parameters.key);
            	break;
            	case "addmode":
            		if (!obj.parameters.channel) {
        				return connection.sendUTF(JSON.stringify({"code":"error", "message":"Channel to join required!", "received":message.utf8Data}));
            		}
            		context.Server.Connection.Write("MODE " + obj.parameters.destination + " +" + obj.parameters.mode + " " + obj.parameters.arguments);
            	break;
            	case "delmode":
            		if (!obj.parameters.channel) {
        				return connection.sendUTF(JSON.stringify({"code":"error", "message":"Channel to join required!", "received":message.utf8Data}));
            		}
            		context.Server.Connection.Write("MODE " + obj.parameters.destination + " -" + obj.parameters.mode + " " + obj.parameters.arguments);
            	break;
            	case "quit":
            		context.Server.Connection.Write("QUIT " + obj.parameters.reason || "dabbit IRC Client. Visit http://dabb.it/ for more information!");
            	break;
            	case "sync":
            		connection.sendUTF(JSON.stringify(context.Server, censor(context.Server)));
            	break;
            }
            //connection.sendUTF(message.utf8Data);
        }
    });

	connection.on('close', function(me)
	{
		Console.WriteLine("Connection closed");
	});

});
