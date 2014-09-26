﻿var System = require("../../System")(__dirname + "/../../System");
var Server = {}; //require("./Server");
var Topic = require("./Topic");

function Channel(svr) {
    // Create an object
    System.Object.call(this, this);
    console.log(svr.GetType());
    if (!svr || svr.GetType() != "Server") {
        throw new System.ArgumentException("Invalid Server Parameter svr");
    }

    this.Name = String.Empty;
    this.Modes = [];
    this.Users = [];

    this.__defineGetter__("Display", function(){
        return this.Name;
    });
    var display = String.Empty;

    this.__defineGetter__("Created", function(){
        return created;
    });
    var created = new Date();

    this.__defineGetter__("ServerOf", function(){
        return server.Value;
    });

    this.__defineGetter__("ChannelLoaded", function(){
        return !String.IsNullOrEmpty(this.Name) && this.Modes.length != 0 && this.Users.length != 0 && !String.IsNullOrEmpty(this.Display);
    });

    this.__defineGetter__("Topic", function(){
        return !String.IsNullOrEmpty(this.Name) && this.Modes.length != 0 && this.Users.length != 0 && !String.IsNullOrEmpty(this.Display);
    });
    this.__defineSetter__("Topic", function(){
        return !String.IsNullOrEmpty(this.Name) && this.Modes.length != 0 && this.Users.length != 0 && !String.IsNullOrEmpty(this.Display);
    });
    var topic = new System.Javascript.CheckedProperty(new Topic(), System.Typeof(Topic));

    this.toString = function()
    {
        return this.Name;
    }

    var server = new System.Javascript.CheckedProperty(svr, System.Typeof(Server));

}
System.Javascript.Inherit(System.Object, Channel);

module.exports = Channel;
