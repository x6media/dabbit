
var Object = require('../Object');
var ArgumentException = require('../ArgumentException');
var Inherit = require('./Inherit');
var Typeof = require('../Typeof.js');

function CheckedProperty(def, getMethodOrType, setMethod, optionalType) {

	Object.call(this);


    if (!setMethod) {
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

    if (!determineValidObject(value, true)) {
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

    	if (determineValidObject(val, true)) {
    		value = val;
    	}
    	else {
    		throw new ArgumentException("Invalid object set");
    	}
    });

    var determineValidObject(inval, allowUndefined) {
        if (!allowUndefined && !inval) return false;

        if (! (inval instanceof this.type)) {
            return false;
        }

        return true;
    }
}

Inherit(Object, CheckedProperty);

//exports.System = { "Javascript": { "CheckedProperty" : CheckedProperty } };
module.exports = CheckedProperty;