require('./Javascript/g_ObjectExtensions');
require('./Javascript/g_StringExtensions');

var Typeof = require('./Typeof.js');

var Exception = require('./Exception');
var Inherit = require('./Javascript/Inherit');


function ArgumentException(sMethod)
{
	Exception.call(this, "Fatal", "The method {0} must be created in all inherited objects".format(sMethod));
}
Inherit(Exception, ArgumentException);

module.exports = ArgumentException;