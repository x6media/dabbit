var readline = require('readline');
var fs = require("fs");

require('./Javascript/g_ObjectExtensions');
require('./Javascript/g_StringExtensions');


var Object = require('./Object');
var Inherit = require('./Javascript/Inherit');

function Console() {
    Object.call(this, this);
}

Console.WriteLine = function()
{
	var msg = [].shift.apply(arguments);

	if (typeof msg == "string")
	{
		Console.Out.write(msg.format(arguments) + "\r\n");
	}
	else
	{
		Console.Out.write((msg || "") + "\r\n");
	}
}
Console.Write = function()
{
	var msg = [].shift.apply(arguments);
	if (typeof msg == "string")
	{
		Console.Out.write(msg.format(arguments));
	}
	else
	{
		Console.Out.write(msg);
	}
}

Console.ReadLine = function(message)
{

	Console.Write(message);
	var buffer = new Buffer(4096);
	var read = fs.readSync(Console.In.fd, buffer, 0, 4096);
	// read in the buffer -2 because of the enter key adding a newline
	return buffer.toString('utf8', 0, read-2);

}

Console.Read = Console.ReadKey = function()
{
	var buffer = new Buffer(4096);
	var read = fs.readSync(Console.In.fd, buffer, 0, 1);
	// read in the buffer -2 because of the enter key adding a newline
	return buffer.toString('utf8', 0, read);
}

Console.In = process.stdin;

Console.Out = process.stdout;
Console.data = String.Empty;
Console.dataRead = false;

Console.SetIn = function(newIn)
{
	Console.In = newIn;
}

Console.SetOut = function(newOut)
{
	Console.Out = newOut;
}

Inherit(Object, Console);

module.exports = Console;