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

namespace Dabbit.Base
{
    public abstract class IrcFromEntity : IrcEntity
    {
        /// <summary>
        /// Creates a new IrcFromEntity
        /// </summary>
        /// <param name="displayName">The name to display for this instance</param>
        public IrcFromEntity(Type fromType, string displayName) : base(displayName) 
        {
            if (fromType == null)
            {
                throw new ArgumentNullException("fromType");
            }

            this.fromType = fromType;
        }

        /// <summary>
        /// Use this to cast this from Entity as a type.
        /// </summary>
        public Type FromType
        {
            get
            {
                return this.fromType;
            }
        }
        Type fromType = null;
    }
}
