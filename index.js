"use strict";

var restify = require("restify");
var http = require('http');
var fs = require("fs");
var WebSocketServer = require('websocket').server;

var System = new require("all")("System");
var Console = System.Console;

var dabbit = new require("all")("dabbit");



/*
var server = restify.createServer();
server.use(restify.bodyParser());

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

	res.send(JSON.stringify({ "wsurl":"wss://jupiter.dabberz.com:8080/ws/7N2n9cOlk0sa8jTGf6NdpexUp6YCdzQJ"}));

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


	var channels = ["#dab"];


	
    connection.on('message', function(message) {
        if (message.type === 'utf8') {
            console.log('Received Message: ' + message.utf8Data);
            connection.sendUTF(message.utf8Data);
        }
    });

	connection.on('close', function(me)
	{
		Console.WriteLine("Connection closed");
	});

});

*/