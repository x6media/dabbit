/**
 * Copyright David Barajas 2013
 * 
 * @Version v1.0
 * @Author dab
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DabbitClient.Base
{
    public abstract class IrcFromEntity : IrcEntity
    {
        /// <summary>
        /// Creates a new IrcFromEntity
        /// </summary>
        /// <param name="displayName">The name to display for this instance</param>
        public IrcFromEntity(string displayName) : base(displayName) 
        {
        }
    }
}
