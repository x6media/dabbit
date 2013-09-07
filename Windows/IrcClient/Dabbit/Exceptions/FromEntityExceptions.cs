using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dabbit.Exceptions
{
    public class UserEntityNoHostException : Exception
    {
        public UserEntityNoHostException(string fullDisplay)
            : base(String.Format("{0} does not have a reported hostname", fullDisplay))
        {
        }
    }
}
