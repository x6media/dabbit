/**
 * Copyright David Barajas 2013
 * 
 * This file defines an IRC Entity. Any item that allows a display
 * property should extend this class.
 * 
 * @Version v1.0
 * @Author dab
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DabbitClient.Base
{
    public abstract class IrcEntity
    {
        /// <summary>
        /// Constructor for IrcEntity. Base class for anything that needs to have a method for getting
        /// the display for output.
        /// </summary>
        /// <param name="displayName">The Displayname of the IRC Entity</param>
        public IrcEntity(string displayName)
        {
            if (String.IsNullOrEmpty(displayName))
            {
                throw new ArgumentNullException("displayName");
            }

            this.displayName = displayName;
        }

        public string Display 
        { 
            get { return this.displayName; } 
        }

        private string displayName = String.Empty;
    }
}
