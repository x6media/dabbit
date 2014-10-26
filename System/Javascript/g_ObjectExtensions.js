if (!Object.prototype.getName) {
	Object.prototype.getName = function() { 
	   var funcNameRegex = /function (.{1,})\(/;
	   var results = (funcNameRegex).exec((this).constructor.toString());
	   return (results && results.length > 1) ? results[1] : "";
	};

	Object.defineProperty(Object.prototype, 'getName', { 
		enumerable: false, 
		value: Object.prototype.getName 
	});
}

if (!Object.prototype.GetType) {
	Object.prototype.GetType = function() { 
	   return this.getName().toLowerCase();
	};
	Object.defineProperty(Object.prototype, 'GetType', { 
		enumerable: false, 
		value: Object.prototype.GetType
	});
}

if (!Object.prototype.Add) {
	// A method to make C# converting less painful
	Object.prototype.Add = function(k,v) { this[k] = v; }
	Object.defineProperty(Object.prototype, 'Add', { 
		enumerable: false, 
		value: Object.prototype.Add 
	});
}

