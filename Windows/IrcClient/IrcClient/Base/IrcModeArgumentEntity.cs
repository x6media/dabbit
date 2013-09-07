using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DabbitClient.Base
{
    class IrcModeArgumentEntity : IrcEntity
    {
        /// <summary>
        /// Creates a new Entity that can represent a Mode argument (Person, or text)
        /// </summary>
        /// <param name="displayName">The Display Name of the argument (User's nick, mode parameter)</param>
        public IrcModeArgumentEntity(string displayName)
            : base(displayName)
        {
        }
    }
}
