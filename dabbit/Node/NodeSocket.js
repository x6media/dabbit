var System = require("all")("System");
var Base = require("all")("dabbit/Base");

function NodeSocket(host, port, ssl) {
    // Indicates object inheritance.
    Base.SocketWrapper.call(this);
    
    var socket = undefined;
    var backlog = String.Empty;
    var rdCb = undefined;
    this.__defineGetter__("Host", function() {
        return host || "irc.gamergalaxy.net";
    });

    this.__defineGetter__("Port", function() {
        return port || 6697;
    });

    this.__defineGetter__("Secure", function() {
        return ssl || true;
    });

    this.__defineGetter__("Connected", function() {
        return (socket ? socket.Connected : false);
    });

    this.ConnectAsync = function(rawData) { 
        rdCb = rawData;  
    };

    this.Disconnect = function() {
        if (this.Connected) {

        }
    }

    // http://stackoverflow.com/a/10012306/486058
    var onData = function(data) {
        backlog += data
        var n = backlog.indexOf('\n')
        // got a \n? emit one or more 'line' events
        while (~n) {
            //stream.emit('line', backlog.substring(0, n))
            rdCb(backlog.substring(0, n));
            backlog = backlog.substring(n + 1)
            n = backlog.indexOf('\n')
        }

    }

    this.__defineGetter__("Reader", function() {
        throw new System.MustImplementException("CreateConnection");
    });

    this.__defineGetter__("Writer", function() {
        throw new System.MustImplementException("CreateConnection");
    });
}
System.Javascript.Inherit(Base.SocketWrapper, NodeSocket);

module.exports = NodeSocket;
