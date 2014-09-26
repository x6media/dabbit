if (!Object.prototype.getName) {
	Object.prototype.getName = function() { 
	   var funcNameRegex = /function (.{1,})\(/;
	   var results = (funcNameRegex).exec((this).constructor.toString());
	   return (results && results.length > 1) ? results[1] : "";
	};
}

if (!Object.prototype.IsInherited) {
	// Useful when determining if an object is inherited
	Object.prototype.IsInherited = function() { return false; }
}
