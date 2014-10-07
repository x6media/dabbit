
var System = require("all")("System");
var Base = require("all")("dabbit/Base");
var NodeSocket = require('./NodeSocket');

function NodeContext() {
    // Indicates object inheritance.
    Base.IContext.call(this);

    /// <summary>
    /// Create a connection object given ConnectionType and ISocketWrapper
    /// </summary>
    this.CreateConnection = function(connectionType, socket) {
        var connection = new Base.Connection(this, socket);
                
        return connection;
    }
    
    this.Servers = [];

    this.Settings = {};
    
    /// <summary>
    /// Creates a blank TCP Socket for use in a connection
    /// </summary>
    /// <param name="host">The host you want to connect to.</param>
    /// <param name="port">port</param>
    /// <param name="secure">bool is secure</param>
    /// <returns>A socket wrapper for a connection. This is platform dependant. This will not be in the same namespace</returns>
    this.CreateSocket = function(host, port, secure) {
        return NodeSocket(host, port, secure);
    }

    /// <summary>
    /// Create a channel object
    /// <param name="svr">The Server object for applying this to. Used for passing into the Channel base class</param>
    /// <returns>An object that either is, or inherits from Channel</returns>
    this.CreateChannel = function(svr) {
        return new Base.Channel(svr);
    }

    /// <summary>
    /// Create a channel object
    /// <param name="source">(OPTIONAL) The SourceEntity object we want to create the user from. If no SourceEntity is passed, we pass an empty user</param>
    /// <returns>An object that either is, or inherits from User</returns>
    this.CreateUser = function(source) {
        return new Base.User(source);
    }
}
System.Javascript.Inherit(Base.IContext, NodeContext);

module.exports = NodeContext;