using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Dabbit.Base;
using Dabbit.Network;
using Dabbit.Exceptions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Dabbit.UnitTests")]
namespace Dabbit.Irc
{
    public class IrcCentral
    {

        /// <summary>
        /// static constructor is used for indicating to the compiler how to handle the constructor
        /// </summary>
        static IrcCentral()
        {
        }
        
        /// <summary>
        /// Private constructor for this singleton pattern
        /// </summary>
        private IrcCentral()
        {
        }

        /// <summary>
        /// Fetches an instance of the class. Call this once you've already set the context
        /// </summary>
        /// <returns>Singleton IrcCentral instance</returns>
        public static IrcCentral Instance()
        {
            if (IrcCentral.instance.Context == null)
            {
                throw new UninitializedException("IIrcContext");
            }

            return IrcCentral.instance;
        }

        /// <summary>
        /// Fetches an instance while setting the context.
        /// </summary>
        /// <param name="context">The Context for this program (supplies provider factories)</param>
        /// <returns>IrcCentral instance</returns>
        public static IrcCentral Instance(IIrcContext context)
        {
            IrcCentral.instance.context = context;
            IrcCentral.instance.networks = new Dictionary<string, IrcNetwork>();
            IrcCentral.instance.settings = new Dictionary<string, string>();

            return IrcCentral.instance;
        }

        /// <summary>
        /// A list of all the IRC networks, connected or not
        /// </summary>
        public Dictionary<string, IrcNetwork> Networks
        {
            get
            {
                return this.networks;
            }
        }

        /// <summary>
        /// Add an IRC network to the list as long as the key isn't already used
        /// </summary>
        /// <param name="key">Name to call the network</param>
        /// <param name="value">The IrcNetwork instance</param>
        public void AddNetwork(string key, IrcNetwork value)
        {
            if (this.networks.ContainsKey(key))
            {
                throw new KeyAlreadyExistsException("network", key);
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            this.networks.Add(key, value);
        }

        /// <summary>
        /// Delete an IRC network from the list by the key
        /// </summary>
        /// <param name="key">The display name of the irc network to remove</param>
        public void DelNetwork(string key)
        {
            if (!this.networks.ContainsKey(key))
            {
                throw new KeyNotExistsException("Network", key);
            }

            this.networks.Remove(key);
        }

        /// <summary>
        /// A list of the settings currently in place
        /// </summary>
        public Dictionary<string, string> Settings
        {
            get { return this.settings; }
        }

        /// <summary>
        /// Add a new setting to the list or replaces an existing key.
        /// </summary>
        /// <param name="key">Name of the setting</param>
        /// <param name="value">Value of the setting. This may change...</param>
        public void SetSetting(string key, string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            
            if (this.networks.ContainsKey(key))
            {
                this.settings[key] = value;
                return;
            }

            this.settings.Add(key, value);
        }

        /// <summary>
        /// Remove a setting from the list
        /// </summary>
        /// <param name="key"></param>
        public void DelSetting(string key)
        {
            if (!this.settings.ContainsKey(key))
            {
                throw new KeyNotExistsException("Network", key);
            }

            this.settings.Remove(key);
        }

        /// <summary>
        /// Fetch an instance of the current Context
        /// </summary>
        public IIrcContext Context
        {
            get
            {
                return new IrcContext();
            }
        }

        /// <summary>
        /// Internal test method to reset the instance to a default state.
        /// </summary>
        /// <param name="context"></param>
        internal void ResetState(IIrcContext context)
        {
            this.context = context;
            this.networks = new Dictionary<string, IrcNetwork>();
            this.settings = new Dictionary<string, string>();
        }

        private static IrcCentral instance = new IrcCentral();
        protected IIrcContext context = null;
        protected Dictionary<string, IrcNetwork> networks = null;
        protected Dictionary<string, string> settings = null;
    }
}
