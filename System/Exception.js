require('./Javascript/g_ObjectExtensions');
require('./Javascript/g_StringExtensions');


var Object = require('./Object');
var Inherit = require('./Javascript/Inherit');


function Exception(sLevel, sMessage)
{
	Object.call(this, this);
	
	var str = arguments.callee.caller.name + "";
	var isInheritedBy = this instanceof Exception && !String.IsNullOrEmpty(str);

	console.log();
	var name = (isInheritedBy ? str : "Exception");
	var lvl = sLevel || "Low";
	var msg = sMessage || "A" + (['a', 'e', 'i', 'o', 'u'].indexOf(name[0].toLowerCase()) == -1 ? " " : "n ") + name + " has occured";
	var stack = new Error().stack;

	this.toString = function()
	{
		return name +": " + msg;
	}

    this.__defineGetter__("Name", function(){
        return name;
    });
    this.__defineGetter__("Level", function(){
        return lvl;
    });
    this.__defineGetter__("Message", function(){
        return msg;
    });
    this.__defineGetter__("StackTrace", function(){
        return stack;
    });


	this.Variable = "omg";
}
Inherit(Object, Exception);

module.exports = Exception;