using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dabbit.Base;

namespace Dabbit.Irc
{
    class IrcModeEntity : IrcEntity
    {
        
        /// <summary>
        /// Creates a new IrcMode object that represents an IRC Mode.
        /// </summary>
        /// <param name="clientDisplay">The display of the IRC mode (~&@%+)</param>
        /// <param name="protocolDisplay">The IRC representation of the mode (qaohv)</param>
        /// <param name="value">The value of the mode for sorting reasons (54321)</param>
        public IrcModeEntity(string clientDisplay, string protocolDisplay, int value) 
            : base(clientDisplay)
        {
            if (String.IsNullOrEmpty(protocolDisplay))
            {
                throw new ArgumentNullException("mode");
            }
            
            this.protocolDisplay = protocolDisplay;
            this.modeValue = value;
        }

        /// <summary>
        /// Creates a new IrcMode object that represents an IRC Mode with an argument
        /// </summary>
        /// <param name="clientDisplay">The display of the IRC mode (~&@%+)</param>
        /// <param name="protocolDisplay">The IRC protocol representation of the mode (qaohv)</param>
        /// <param name="value">The value of the mode for sorting reasons (54321)</param>
        /// <param name="modeArgument">If a mode has an argument, include it here (ie: +k modeArgument)</param>
        public IrcModeEntity(string clientDisplay, string protocolDisplay, int value, IrcModeArgumentEntity modeArgument)
            : this(clientDisplay, protocolDisplay, value)
        {
            if (modeArgument == null)
            {
                throw new ArgumentNullException("modeArgument");
            }

            this.modeHasArgument = true;
            this.modeArgument = modeArgument;
        }

        /// <summary>
        /// Returns the protocol character for this mode
        /// </summary>
        public string IrcDisplay
        {
            get { return this.protocolDisplay; }
        }

        /// <summary>
        /// Gets the value for this mode (ie q == 5, a == 4, etc..)
        /// </summary>
        public int Value
        {
            get { return this.modeValue; }
        }

        /// <summary>
        /// Returns the argument provided with this mode (+k password, the argument would contain password). Can return null
        /// if this.HasRgument returns false
        /// </summary>
        public IrcModeArgumentEntity Argument
        {
            get { return this.modeArgument; }
        }

        /// <summary>
        /// Determines if this mode contains an argument
        /// </summary>
        public bool HasArgument
        {
            get { return this.modeHasArgument; }
        }

        private string protocolDisplay = String.Empty;
        private int modeValue = 0;
        private bool modeHasArgument = false;
        private IrcModeArgumentEntity modeArgument = null;
    }
}
