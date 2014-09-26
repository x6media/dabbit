// Require some javascript prototpye global helpers
require('./g_StringExtensions');
require('./g_ObjectExtensions');

/*


// Import all our System.* objects
var Object = require('./Object').System.Object;
var Exception = require('./Exception').System.Exception;
var Console = require('./Console').System.Console;

// Import all our Javascript.*
var Javascript = require('./Javascript/Javascript');

var System = {
	// System.* Items
	"Object" : Object, 
	"Console" : Console,
	"Exception" : Exception,

	// System.x.* items
	"Javascript": Javascript
};


exports.System = System;

*/
var fs = require('fs');
var path = require('path');

var curdir = path.join(__dirname, "/");

fs.readdirSync(curdir).forEach(function(file) {
	var fullPath = path.join(curdir, file);

	if (fs.statSync(fullPath).isDirectory()) {
		exports[file] = require(path.join(fullPath, "index.js"));
	}
	else if (file.match(/.+\.js/g) !== null && file !== 'index.js' && ! file.startsWith("g_") ) {
		var name = file.replace('.js', '');
		exports[name] = require('./' + file);
	}
});