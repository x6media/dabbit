using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dabbit.Base;

namespace Dabbit.Network
{
    public delegate void SendDelegate(string message);
    public delegate Guid ConnectDelegate(IrcConnection connectionDetails);
    public delegate void DisconnectDelegate();
    
    public interface IIrcSocket : IIrcProvider
    {
        /// <summary>
        /// Send a message to the IRC socket
        /// </summary>
        /// <param name="message"></param>
        void Send(string message);

        /// <summary>
        /// Connect to the Network
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnect from the Network
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Is the socket connected to the network
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// The hostname this socket is connected to
        /// </summary>
        string Hostname { get; set; }

        /// <summary>
        /// The port the socket is connected to
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// If this connection uses SSL, this returns true
        /// </summary>
        bool SSL { get; set; }

        /// <summary>
        /// The connection ID if applicable.
        /// </summary>
        Guid ConnectionId { get; set; }

        /// <summary>
        /// This is set by the client's socket class
        /// Used to allow the library to communicate with 
        /// an IRC socket connection
        /// </summary>
        SendDelegate SendDelegate { set; }

        /// <summary>
        /// ReceiveDelegate is called by the client's socket class when
        /// data is received. The client passes the info into the library via this method.
        /// </summary>
        void ReceiveDelegate(string message);

        /// <summary>
        /// When the library wishes to perform a connect, this library calls this method
        /// and attempts to send the connection details to the client's socket class.
        /// </summary>
        ConnectDelegate ConnectDelegate { set; }

        /// <summary>
        /// Disconnect from the server. The library calls this method when it wishes to close the
        /// connection. The client socket should implement this method to be called.
        /// </summary>
        DisconnectDelegate DisconnectDelegate { set; }

        /// <summary>
        /// When the server disconnects the client, this method is called to inform the 
        /// library.
        /// </summary>
        void DisconnectedDelegate();
    }

    public class IrcSocket : IIrcSocket
    {
        /// <summary>
        /// Send a message to the IRC socket
        /// </summary>
        /// <param name="message"></param>
        public void Send(string message)
        {

        }

        /// <summary>
        /// Connect to the Network
        /// </summary>
        public void Connect()
        {
            
        }

        /// <summary>
        /// Disconnect from the Network
        /// </summary>
        public void Disconnect()
        {

        }
        
        /// <summary>
        /// Is the socket connected to the network
        /// </summary>
        public bool Connected { get { return true; } }

        /// <summary>
        /// The hostname this socket is connected to
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// The port the socket is connected to
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// If this connection uses SSL, this returns true
        /// </summary>
        public bool SSL { get; set; }

        /// <summary>
        /// The connection ID if applicable.
        /// </summary>
        public Guid ConnectionId { get; set; }

        /// <summary>
        /// This is set by the client's socket class
        /// Used to allow the library to communicate with 
        /// an IRC socket connection
        /// </summary>
        public SendDelegate SendDelegate
        {
            set
            {
                this.sendDelegate = value;
            }
        }

        /// <summary>
        /// When the library wishes to perform a connect, this library calls this method
        /// and attempts to send the connection details to the client's socket class.
        /// </summary>
        public ConnectDelegate ConnectDelegate { set { this.connectDelegate = value; } }

        /// <summary>
        /// Disconnect from the server. The library calls this method when it wishes to close the
        /// connection. The client socket should implement this method to be called.
        /// </summary>
        public DisconnectDelegate DisconnectDelegate { set { this.disconnectDelegate = value; } }
        
        public void ReceiveDelegate(string message)
        {

        }

        public void DisconnectedDelegate()
        {

        }

        private SendDelegate sendDelegate = null;
        private ConnectDelegate connectDelegate = null;
        private DisconnectDelegate disconnectDelegate = null;
    }
}
