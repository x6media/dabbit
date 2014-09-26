// Require some javascript prototpye global helpers
require('./Javascript/g_StringExtensions');
require('./Javascript/g_ObjectExtensions');

function requireall(basePath)
{
	var fs = require('fs');
	var path = require('path');

	var curdir = path.join(basePath, "/");
	var tmpexport = {};
	fs.readdirSync(curdir).forEach(function(file) {

		var fullPath = path.join(curdir, file);

		if (fs.statSync(fullPath).isDirectory()) {
			tmpexport[file] = requireall(fullPath);
		}
		else if (file.endsWith(".js") && file !== 'index.js' && ! file.startsWith("g_") ) {

			var name = file.replace('.js', '');
			tmpexport[name] = require(fullPath);
		}
	});

	return tmpexport;

}

module.exports = requireall;