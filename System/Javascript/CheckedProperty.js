
var Object = require('../Object');
var ArgumentException = require('../ArgumentException');
var Inherit = require('./Inherit');
var Typeof = require('../Typeof.js');

function CheckedProperty(def, getMethodOrType, setMethod, optionalType) {

	Object.call(this);


    if (typeof getMethodOrType == "string") {
    	this.type = getMethodOrType;
    	this.setMethod = function() { return true; };
    	this.setMethod = function() { return true; };
    }
    else {
    	this.getMethod = (getMethodOrType || function() { return true; });
    	this.setMethod = (setMethod || function() { return true; });
    	this.type = optionalType || "Object";
	}

    var value = def;

    if (this.type != "Object" && val && Typeof(val) != this.type) {
        throw new ArgumentException("Invalid object set");
    }

    this.__defineGetter__("Value", function() {
    	var res =  this.getMethod(value);

        return (typeof res != "boolean" ? res : value);
    });

    this.__defineSetter__("Value", function(val) {
    	var result = this.setMethod(value, val);

    	// If our user passed function returned false
		if (typeof result == "boolean" && !result) {
    		return;
    	}

    	if (result[0]) {
    		val = result.instead;
    	}

    	if (this.type == "Object" || !val || Typeof(val) == this.type) {
    		value = val;
    	}
    	else {
    		throw new ArgumentException("Invalid object set");
    	}
    });
}

Inherit(Object, CheckedProperty);

//exports.System = { "Javascript": { "CheckedProperty" : CheckedProperty } };
module.exports = CheckedProperty;