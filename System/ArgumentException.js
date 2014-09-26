require('./Javascript/g_ObjectExtensions');
require('./Javascript/g_StringExtensions');

var Typeof = require('./Typeof.js');

var Exception = require('./Exception');
var Inherit = require('./Javascript/Inherit');


function ArgumentException(sMessage)
{
	Exception.call(this, "Fatal", "An argument error occured: {0}".format(sMessage));
}
Inherit(Exception, ArgumentException);

module.exports = ArgumentException;