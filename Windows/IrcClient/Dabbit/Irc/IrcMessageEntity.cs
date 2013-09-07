using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dabbit.Base;

namespace Dabbit.Irc
{

    class IrcMessageEntity : IrcEntity
    {

        /// <summary>
        /// create a new IRC Message Entity
        /// </summary>
        /// <param name="fromOrigin">The user/server that this message originated from</param>
        /// <param name="toUser">The destination of this message</param>
        /// <param name="messageParts">The message broken down into a string array</param>
        IrcMessageEntity(IrcFromEntity fromOrigin, IrcFromEntity toUser, IEnumerable<string> messageParts)
            : base(String.Join(" ", messageParts))
        {
            if (fromOrigin == null)
            {
                throw new ArgumentNullException("fromUser");
            }

            if (toUser == null)
            {
                throw new ArgumentNullException("toUser");
            }

            this.fromOrigin = fromOrigin;
            this.toUser = toUser;
            this.messageParts = messageParts;
        }

        /// <summary>
        /// The user or server that this message came from
        /// </summary>
        public IrcFromEntity Origin
        {
            get { return this.fromOrigin; }
        }

        /// <summary>
        /// The destination 
        /// </summary>
        public IrcFromEntity Destination
        {
            get { return this.toUser; }
        }

        /// <summary>
        /// The pieces of the message broken into an array
        /// </summary>
        public IEnumerable<string> MessageParts
        {
            get { return this.messageParts; }
        }

        private IrcFromEntity fromOrigin = null;
        private IrcFromEntity toUser = null;
        private IEnumerable<String> messageParts = null;
    }
}
