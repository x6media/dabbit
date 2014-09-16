var restify = require("restify");

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

	res.send(JSON.stringify({ "wsurl":"wss://jupiter.dabberz.com:443/7N2n9cOlk0sa8jTGf6NdpexUp6YCdzQJ"}));
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

	res.send(JSON.stringify({ "result":"success", "messages":[ {"key":"login", "value":"Authentication successfull"}]}));
	next();
}

function register(req, res, next)
{
  res.send('hello ');
  next();
}

function startSession(req, res, next)
{
	  res.send('hello ');
  next();
}

function extendSession(req, res, next)
{
	  res.send('hello ');
  next();
}

function subscribePushNotification(req, res, next)
{
	  res.send('hello ');
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


// First, checks if it isn't implemented yet.
if (!String.prototype.format) {
  String.prototype.format = function() {


    var args = (arguments.length > 0 && typeof arguments[0] != "string" ? arguments[0] : arguments);
    
    return this.replace(/{(\d+)}/g, function(match, number) { 
      return typeof args[number] != 'undefined'
        ? args[number]
        : match
      ;
    });
  };
}
Object.prototype.getName = function() { 
   var funcNameRegex = /function (.{1,})\(/;
   var results = (funcNameRegex).exec((this).constructor.toString());
   return (results && results.length > 1) ? results[1] : "";
};



function consl() {
}

consl.prototype.WriteLine = function()
{
	var msg = [].shift.apply(arguments);

	console.log(msg.format(arguments));
}

var Console = new consl();

apiCalls.forEach(function(val, ind, ar) 
	{ 
		server.post("/" + val.name, val.func); 
		Console.WriteLine("Added api {0}", val.name);
	}
);

server.pre(restify.pre.userAgentConnection());

server.listen(8080);