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

var context = new dabbit.Node.NodeContext();

var me = new dabbit.Base.User();
me.Nick = "dabbit";
me.Ident = "dabitp";
me.Name = "David";


context.AddServer(me, context.CreateConnection(dabbit.Base.ConnectionType.Direct, context.CreateSocket(ircServer, ircPort, ircSsl)));

context.Server.Events.on("OnRawMessage", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnChannelAction", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnChannelMessage", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnQueryAction", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnQueryMessage", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnChannelActionNotice", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnChannelMessageNotice", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnQueryActionNotice", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnQueryMessageNotice", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnError", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnNewChannelJoin", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnJoin", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnPart", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnCloseChannelPart", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnKick", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnNickChange", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnBan", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnUnban", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnModeChange", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnNewChannelJoin", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnTopic", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnAway", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnUnAway", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnInvite", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnWhoIs", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnConnectionEstablished", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnMotd", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnList", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.Events.on("OnUnhandledEvent", function(svr, msg) { console.log(JSON.stringify(msg)); } );
context.Server.PerformConnect();

/*
var apiCalls = [ 
	{"name": "createConnection", "func":createConnection},
	{"name": "login", "func":login},
	{"name": "register", "func":register},
	{"name": "startSession", "func":startSession},	
	{"name": "extendSession", "func":extendSession},
	{"name": "subscribePushNotification", "func":subscribePushNotification},
];

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

            	break;
            	case "connect":
            		if (!obj.parameters.nick || !obj.parameters.ident || !obj.parameters.real) {
        				 return connection.sendUTF(JSON.stringify({"code":"error", "message":"Missing nick, ident, or real name", "received":message.utf8Data}));
            		}
            		var me = new dabbit.Base.User();
            		me.Nick = obj.parameters.nick;
            		me.Ident = obj.parameters.ident;
            		me.Name = obj.parameters.real;

            		context.AddServer(me, context.CreateConnection(dabbit.Base.ConnectionType.Direct, context.CreateSocket(ircServer, ircPort, ircSsl)));
            		this.Server.Events.on("OnPrivmsg", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnNotice", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnAction", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnMode", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnCtcp", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnError", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnPing", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnKick", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnJoin", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnNames", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnList", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnPart", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnInvite", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnBan", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnUnban", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnTopic", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnNick", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnQuit", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnAway", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnWhois", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnMotd", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnPong", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnRaw", function(svr, msg) { console.log(msg); } );
            		this.Server.Events.on("OnConnect", function(svr, msg) { console.log(msg); } );
        			this.Server.PerformConnect();

            	break;
            	case "msg":

            	break;
            	case "join":

            	break;
            	case "part":

            	break;
            	case "addmode":

            	break;
            	case "delmode":


            	break;
            	case "quit":

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
*/